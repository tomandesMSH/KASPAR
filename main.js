const { app, BrowserWindow, ipcMain, shell, dialog } = require('electron');
const { execFile, spawn, spawnSync } = require('child_process');
const { autoUpdater } = require('electron-updater');
const log = require('electron-log');
const path = require('path');
const fs   = require('fs');

// ─── Admin check & relaunch ───────────────────────────────────────────────────
function isAdmin() {
  try {
    const result = spawnSync('net', ['session'], { windowsHide: true });
    return result.status === 0;
  } catch {
    return false;
  }
}

function relaunchAsAdmin() {
  const exe  = process.execPath;
  const args = app.isPackaged ? [] : [app.getAppPath()];
  try {
    spawnSync('powershell.exe', [
      '-NoProfile', '-NonInteractive', '-Command',
      `Start-Process -FilePath ${JSON.stringify(exe)}` +
      (args.length ? ` -ArgumentList ${JSON.stringify(args.join(' '))}` : '') +
      ' -Verb RunAs'
    ], { windowsHide: true });
    return true;
  } catch {
    return false;
  }
}

// If we already tried elevation and user clicked No on UAC, this flag is set
const USER_DECLINED_ADMIN = process.argv.includes('--no-admin');

// ─── Auto-updater logging ─────────────────────────────────────────────────────
autoUpdater.logger = log;
autoUpdater.logger.transports.file.level = 'info';
autoUpdater.autoDownload = false;

// ─── Auto-updater events ──────────────────────────────────────────────────────
autoUpdater.on('update-available', (info) => {
  dialog.showMessageBox(mainWin, {
    type: 'info',
    title: 'Update Available',
    message: `KASPAR v${info.version} is available.`,
    detail: 'A new version was detected. Do you want to download and install it now?',
    buttons: ['Download and Install', 'Later'],
    defaultId: 0,
  }).then(({ response }) => {
    if (response === 0) autoUpdater.downloadUpdate();
  });
});

autoUpdater.on('update-downloaded', () => {
  dialog.showMessageBox(mainWin, {
    type: 'info',
    title: 'Update Ready',
    message: 'Update downloaded.',
    detail: 'KASPAR will restart and install the update now.',
    buttons: ['Restart Now'],
    defaultId: 0,
  }).then(() => {
    autoUpdater.quitAndInstall();
  });
});

const SPLASH_MIN_MS = 1500;

// ─── Paths ────────────────────────────────────────────────────────────────────
function scriptsRoot() {
  return app.isPackaged
    ? path.join(process.resourcesPath, 'scripts')
    : path.join(__dirname, 'scripts');
}

// ─── Splash window ────────────────────────────────────────────────────────────
let splashWin = null;

function createSplash() {
  splashWin = new BrowserWindow({
    width: 480,
    height: 320,
    frame: false,
    resizable: false,
    center: true,
    backgroundColor: '#0d0d0f',
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: false,
    },
  });
  splashWin.loadFile('splash.html');
}

function splashCall(fn) {
  if (splashWin && !splashWin.isDestroyed()) {
    splashWin.webContents.executeJavaScript(fn).catch(() => {});
  }
}

function setSplashStatus(text, type = '') { splashCall(`window.setStatus(${JSON.stringify(text)}, ${JSON.stringify(type)})`); }
function setSplashProgress(pct)           { splashCall(`window.setProgress(${pct})`); }
function setSplashStep(index)             { splashCall(`window.setStep(${index})`); }

// ─── Main window ──────────────────────────────────────────────────────────────
let mainWin = null;

function createMainWindow() {
  mainWin = new BrowserWindow({
    width: 960,
    height: 680,
    minWidth: 760,
    minHeight: 520,
    frame: false,
    show: false,
    backgroundColor: '#0d0d0f',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
    icon: path.join(__dirname, 'assets', 'icon.png'),
  });

  mainWin.loadFile('index.html');

  mainWin.webContents.setWindowOpenHandler(({ url }) => {
    openDocsWindow(url);
    return { action: 'deny' };
  });

  ipcMain.on('win-minimize', () => mainWin.minimize());
  ipcMain.on('win-maximize', () => mainWin.isMaximized() ? mainWin.unmaximize() : mainWin.maximize());
  ipcMain.on('win-close',    () => mainWin.close());
}

// ─── App entry point ──────────────────────────────────────────────────────────
app.whenReady().then(async () => {

  // ── Admin check ─────────────────────────────────────────────────────────────
  if (!isAdmin()) {
    if (!USER_DECLINED_ADMIN) {
      // Try to relaunch elevated — this opens a UAC prompt
      // We pass --no-admin so if it somehow falls back to non-admin we know
      relaunchAsAdmin();
      // Whether UAC was accepted or cancelled, quit this non-admin instance.
      // If accepted: the new elevated instance takes over.
      // If cancelled: nothing launched, so we relaunch ourselves with --no-admin
      // to show the warning instead of looping.
      app.quit();
      return;
    }
    // USER_DECLINED_ADMIN = true: user cancelled UAC, continue without admin and show warning
  }

  // 1. Show splash
  createSplash();

  // 2. Start loading main window
  createMainWindow();

  splashWin.webContents.once('did-finish-load', async () => {
    splashCall(`window.setVersion(${JSON.stringify('v' + app.getVersion())})`);

    const splashStart = Date.now();

    setSplashStep(0);
    setSplashStatus('Loading KASPAR...');
    setSplashProgress(-1);
    await sleep(400);

    setSplashStep(1);
    setSplashStatus('Preparing scripts...');
    setSplashProgress(50);
    await sleep(400);

    setSplashStep(2);
    setSplashStatus('Checking for updates...');
    setSplashProgress(75);
    await sleep(600);

    setSplashStep(3);
    setSplashStatus('Ready!', 'ok');
    setSplashProgress(100);

    const elapsed   = Date.now() - splashStart;
    const remaining = Math.max(0, SPLASH_MIN_MS - elapsed);

    await Promise.all([
      new Promise(resolve => {
        if (mainWin.webContents.isLoading()) {
          mainWin.webContents.once('did-finish-load', resolve);
        } else {
          resolve();
        }
      }),
      sleep(remaining),
    ]);

    mainWin.show();
    await sleep(150);
    if (splashWin && !splashWin.isDestroyed()) {
      splashWin.close();
      splashWin = null;
    }

    // If user declined UAC, notify the renderer so it can show the warning
    if (USER_DECLINED_ADMIN || !isAdmin()) {
      mainWin.webContents.send('admin-warning');
    }

    autoUpdater.checkForUpdatesAndNotify();
  });
});

// ─── Open a URL in a maximized Electron window ───────────────────────────────
function openDocsWindow(url) {
  const docsWin = new BrowserWindow({
    width: 1280,
    height: 800,
    backgroundColor: '#ffffff',
    autoHideMenuBar: true,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: false,
    },
  });
  docsWin.loadURL(url);
  docsWin.maximize();
}

// ─── Open a URL in the system default browser ────────────────────────────────
ipcMain.on('open-external', (_, url) => {
  shell.openExternal(url);
});

// ─── Run a silent script (apply/revert) ──────────────────────────────────────
ipcMain.handle('run-script', (_, exe) => {
  return new Promise((resolve) => {
    const scriptPath = path.join(scriptsRoot(), exe);
    execFile(
      scriptPath,
      [],
      { windowsHide: false, maxBuffer: 10 * 1024 * 1024 },
      (error, stdout, stderr) => {
        if (error && error.code !== 0) {
          resolve({ success: false, message: stderr || error.message });
        } else {
          resolve({ success: true, message: stdout || 'Completed.' });
        }
      }
    );
  });
});

// ─── Run an interactive script (opens its own console window) ────────────────
ipcMain.handle('run-interactive', (_, exe) => {
  const scriptPath = path.join(scriptsRoot(), exe);
  const child = spawn('cmd.exe', ['/c', scriptPath], {
    detached: true,
    stdio: 'ignore',
    windowsHide: false,
  });
  child.unref();
  return { success: true };
});

// ─── Run a script elevated (UAC prompt) ──────────────────────────────────────
ipcMain.handle('run-elevated', (_, exe) => {
  return new Promise((resolve) => {
    const scriptPath = path.join(scriptsRoot(), exe);
    const tmpOut = path.join(app.getPath('temp'), `kaspar_out_${Date.now()}.txt`);

    const psCommand =
      `$p = Start-Process -FilePath ${JSON.stringify(scriptPath)} -Verb RunAs -Wait -PassThru` +
      ` -RedirectStandardOutput ${JSON.stringify(tmpOut)} -RedirectStandardError ${JSON.stringify(tmpOut)};` +
      ` exit $p.ExitCode`;

    const child = spawn('powershell.exe', [
      '-NoProfile', '-NonInteractive', '-Command', psCommand
    ], { windowsHide: true });

    child.on('close', (code) => {
      let output = '';
      try { output = fs.readFileSync(tmpOut, 'utf8').trim(); } catch (_) {}
      try { fs.unlinkSync(tmpOut); } catch (_) {}

      if (code === 0) {
        resolve({ success: true, message: output || 'Completed.' });
      } else if (code === null) {
        resolve({ success: false, message: 'Cancelled: UAC prompt was dismissed.' });
      } else {
        resolve({ success: false, message: output || `Exited with code ${code}` });
      }
    });
  });
});

// ─── Cleanup ──────────────────────────────────────────────────────────────────
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});

// ─── Utility ──────────────────────────────────────────────────────────────────
function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
