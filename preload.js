const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  // Run a script .exe from the /scripts folder
  runScript: (exe) => ipcRenderer.invoke('run-script', exe),

  // Open a URL in the system default browser
  openExternal: (url) => ipcRenderer.send('open-external', url),

  // Window controls
  minimize: () => ipcRenderer.send('win-minimize'),
  maximize: () => ipcRenderer.send('win-maximize'),
  close:    () => ipcRenderer.send('win-close'),
});
