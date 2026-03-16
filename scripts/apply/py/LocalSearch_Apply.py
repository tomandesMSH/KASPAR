import winreg
import sys

paths = [
    (winreg.HKEY_CURRENT_USER,  r"Software\Policies\Microsoft\Explorer"),
    (winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Policies\Microsoft\Windows\Explorer"),
]

try:
    for hive, path in paths:
        key = winreg.CreateKey(hive, path)
        winreg.SetValueEx(key, "DisableSearchBoxSuggestions", 0, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
