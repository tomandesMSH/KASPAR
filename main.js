const { app, BrowserWindow, ipcMain, shell } = require('electron');
const { spawn } = require('child_process');
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

ipcMain.on('open-external', (_, url) => {
  shell.openExternal(url);
});

// ─── Run a script from ./scripts/ ────────────────────────────────────────────
// Problem: scripts self-elevate via ShellExecuteW runas + sys.exit(), so the
// original process exits immediately and execFile resolves too early, causing
// all scripts to launch at once.
//
// Fix: We use PowerShell's Start-Process with -Verb RunAs -Wait, which:
//   1. Triggers UAC and launches the exe elevated
//   2. Blocks (-Wait) until the elevated process fully closes
//   3. Means the PowerShell process (which we DO track) only exits after
//      the user has finished with the script window and closed it
//
// This guarantees true one-at-a-time sequential execution.
ipcMain.handle('run-script', (_, exe) => {
  return new Promise((resolve) => {
    const scriptPath = path.join(__dirname, 'scripts', exe);

    const ps = spawn(
      'powershell.exe',
      [
        '-NoProfile',
        '-WindowStyle', 'Hidden',
        '-Command',
        `Start-Process -FilePath "${scriptPath}" -Verb RunAs -Wait`,
      ],
      { windowsHide: true }
    );

    let stderr = '';
    ps.stderr.on('data', (data) => { stderr += data.toString(); });

    ps.on('close', (code) => {
      if (code === 0) {
        resolve({ success: true, message: 'Completed.' });
      } else {
        resolve({ success: false, message: stderr || `Exited with code ${code}` });
      }
    });

    ps.on('error', (err) => {
      resolve({ success: false, message: err.message });
    });
  });
});

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
