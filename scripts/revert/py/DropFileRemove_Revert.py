import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

PATH = r"SYSTEM\ControlSet001\Control\FeatureManagement\Overrides\14\3895955085"

try:
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, PATH, 0, winreg.KEY_SET_VALUE)
        winreg.DeleteValue(key, "EnabledState")
        winreg.DeleteValue(key, "EnabledStateOptions")
        winreg.CloseKey(key)
    except FileNotFoundError:
        pass  # Already default, that's fine

except Exception as e:
    sys.exit(1)
