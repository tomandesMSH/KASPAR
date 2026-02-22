import winreg
import sys
import time
path = r"Software\Microsoft\Windows\CurrentVersion\SearchSettings"
value_name = "IsDynamicSearchBoxEnabled"

try:
    try:
        reg_key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, path, 0, winreg.KEY_SET_VALUE)
    except FileNotFoundError:
        reg_key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)

    winreg.SetValueEx(reg_key, value_name, 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(reg_key)

except Exception as e:
    sys.exit(1)
time.sleep(1)