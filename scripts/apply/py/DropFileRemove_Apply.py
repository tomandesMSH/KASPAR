import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

PATH = r"SYSTEM\ControlSet001\Control\FeatureManagement\Overrides\14\3895955085"

try:
    key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, PATH)
    winreg.SetValueEx(key, "EnabledState", 0, winreg.REG_DWORD, 1)
    winreg.SetValueEx(key, "EnabledStateOptions", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
