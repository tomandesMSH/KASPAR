import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

path = r"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked"

key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, path, 0, winreg.KEY_SET_VALUE)
winreg.DeleteValue(key, "{CB3B0003-8088-4EDE-8769-8B354AB2FF8C}")
winreg.DeleteValue(key, "{241F3F77-3B9E-4C64-8E2E-1A7C62C2F1AE}")
winreg.CloseKey(key)