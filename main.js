const { app, BrowserWindow, ipcMain, shell, dialog } = require('electron');
const { execFile, spawn } = require('child_process');
const { autoUpdater } = require('electron-updater');
const log = require('electron-log');
const path = require('path');
const fs   = require('fs');

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

const SPLASH_MIN_MS = 1500; // minimum time splash is visible

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
    show: false,           // hidden until splash is done
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
app.whenReady().then(() => {
  // 1. Show splash immediately
  createSplash();

  // 2. Start loading the main window in the background straight away
  createMainWindow();

  splashWin.webContents.once('did-finish-load', async () => {
    // Show version
    splashCall(`window.setVersion(${JSON.stringify('v' + app.getVersion())})`);

    // Record when the splash appeared so we can enforce the minimum duration
    const splashStart = Date.now();

    // ── Animate through the steps ────────────────────────────────────────────
    setSplashStep(0);
    setSplashStatus('Loading KASPAR...');
    setSplashProgress(-1); // indeterminate shimmer
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

    // ── Wait for main window AND minimum splash duration ─────────────────────
    const elapsed   = Date.now() - splashStart;
    const remaining = Math.max(0, SPLASH_MIN_MS - elapsed);

    await Promise.all([
      // ensure main window has fully loaded
      new Promise(resolve => {
        if (mainWin.webContents.isLoading()) {
          mainWin.webContents.once('did-finish-load', resolve);
        } else {
          resolve();
        }
      }),
      // enforce minimum splash display time
      sleep(remaining),
    ]);

    // ── Transition: show main, close splash ───────────────────────────────────
    mainWin.show();
    await sleep(150); // brief overlap so there's no black gap
    if (splashWin && !splashWin.isDestroyed()) {
      splashWin.close();
      splashWin = null;
    }

    // Check for updates after app is visible
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

// ─── Cleanup ──────────────────────────────────────────────────────────────────
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});

// ─── Utility ──────────────────────────────────────────────────────────────────
function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
