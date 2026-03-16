import winreg
import sys

paths = [
    (winreg.HKEY_CURRENT_USER,  r"Software\Policies\Microsoft\Explorer"),
    (winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Policies\Microsoft\Windows\Explorer"),
]

for hive, path in paths:
    try:
        key = winreg.OpenKey(hive, path, 0, winreg.KEY_SET_VALUE)
        winreg.DeleteValue(key, "DisableSearchBoxSuggestions")
        winreg.CloseKey(key)
    except FileNotFoundError:
        pass  # already gone, that's fine
    except Exception as e:
        print("ERROR:", e)
        sys.exit(1)
