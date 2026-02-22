import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

path = r"SOFTWARE\Microsoft\WindowsUpdate\Orchestrator"

try:
    key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, path, 0, winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY)
    winreg.SetValueEx(key, "ShutdownFlyoutOptions", 0, winreg.REG_DWORD, 0xF)
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
