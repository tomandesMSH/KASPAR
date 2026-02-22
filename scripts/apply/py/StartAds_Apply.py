import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

key_path = r"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"

try:
    key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_SET_VALUE)
    winreg.SetValueEx(key, "SystemPaneSuggestionsEnabled", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

except FileNotFoundError:
    sys.exit(1)
except Exception as e:
    sys.exit(1)
