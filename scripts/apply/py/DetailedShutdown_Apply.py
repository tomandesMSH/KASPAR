import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

path = r"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"

try:
    key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
    winreg.SetValueEx(key, "VerboseStatus", 0, winreg.REG_DWORD, 1)
    winreg.CloseKey(key)
except PermissionError:
    sys.exit(1)
except Exception as e:
    sys.exit(1)
