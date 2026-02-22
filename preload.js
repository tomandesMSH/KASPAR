const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  // Run a silent script (apply/revert) — waits for completion
  runScript: (exe) => ipcRenderer.invoke('run-script', exe),

  // Run an interactive script — opens a console window, returns immediately
  runInteractive: (exe) => ipcRenderer.invoke('run-interactive', exe),

  // Open a URL in the system default browser
  openExternal: (url) => ipcRenderer.send('open-external', url),

  // Window controls
  minimize: () => ipcRenderer.send('win-minimize'),
  maximize: () => ipcRenderer.send('win-maximize'),
  close:    () => ipcRenderer.send('win-close'),
});
