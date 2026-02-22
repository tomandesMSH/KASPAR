import winreg
import sys

path = r"Software\Policies\Microsoft\Explorer"

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
    winreg.SetValueEx(key, "DisableSearchBoxSuggestions", 0, winreg.REG_DWORD, 1)
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
