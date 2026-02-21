const { app, BrowserWindow, ipcMain, shell } = require('electron');
const { execFile } = require('child_process');
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

  // Intercept new-window / target="_blank" links and open them maximized
  win.webContents.setWindowOpenHandler(({ url }) => {
    openDocsWindow(url);
    return { action: 'deny' }; // prevent default Electron handling
  });

  // Window controls
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

// ─── Run a script from ./scripts/ ────────────────────────────────────────────
ipcMain.handle('run-script', (_, exe) => {
  return new Promise((resolve) => {
    const scriptPath = path.join(__dirname, 'scripts', exe);
    execFile(scriptPath, { windowsHide: false }, (error, stdout) => {
      if (error) {
        resolve({ success: false, message: error.message });
      } else {
        resolve({ success: true, message: stdout || 'Completed.' });
      }
    });
  });
});

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
