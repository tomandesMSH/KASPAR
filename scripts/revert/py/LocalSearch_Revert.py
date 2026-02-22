import winreg
import sys

path = r"Software\Policies\Microsoft\Explorer"

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
    try:
        winreg.DeleteValue(key, "DisableSearchBoxSuggestions")
    except FileNotFoundError:
        pass  # Already default, that's fine
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
