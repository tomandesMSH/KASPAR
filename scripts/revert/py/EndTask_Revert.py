import winreg
import sys

PATH = r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"

try:
    try:
        key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, PATH, 0, winreg.KEY_SET_VALUE)
        winreg.DeleteValue(key, "TaskbarEndTask")
        winreg.CloseKey(key)
    except FileNotFoundError:
        pass  # Already default, that's fine

except Exception as e:
    sys.exit(1)
