const { app, BrowserWindow, ipcMain, shell } = require('electron');
const { execFile, spawn } = require('child_process');
const path = require('path');

function createWindow() {
  const win = new BrowserWindow({
    width: 960,
    height: 680,
    minWidth: 760,
    minHeight: 520,
    frame: false,
    backgroundColor: '#0d0d0f',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
    icon: path.join(__dirname, 'assets', 'icon.png'),
  });

  win.loadFile('index.html');

  win.webContents.setWindowOpenHandler(({ url }) => {
    openDocsWindow(url);
    return { action: 'deny' };
  });

  ipcMain.on('win-minimize', () => win.minimize());
  ipcMain.on('win-maximize', () => win.isMaximized() ? win.unmaximize() : win.maximize());
  ipcMain.on('win-close',    () => win.close());
}

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

// ─── Resolve the scripts root ────────────────────────────────────────────────
function scriptsRoot() {
  return app.isPackaged
    ? path.join(process.resourcesPath, 'scripts')
    : path.join(__dirname, 'scripts');
}

// ─── Run a silent script (apply/revert) ──────────────────────────────────────
// Waits for the process to fully exit before resolving — enables sequential runs.
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
// Uses cmd.exe /c so the exe gets a proper visible console.
// Detached so Electron doesn't wait on it — the user closes it themselves.
ipcMain.handle('run-interactive', (_, exe) => {
  const scriptPath = path.join(scriptsRoot(), exe);
  const child = spawn('cmd.exe', ['/c', scriptPath], {
    detached: true,
    stdio: 'ignore',
    windowsHide: false,
  });
  child.unref(); // Let it live independently of Electron
  return { success: true };
});

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
