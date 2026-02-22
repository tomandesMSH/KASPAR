import winreg
import sys

PATH = r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, PATH)
    winreg.SetValueEx(key, "TaskbarEndTask", 0, winreg.REG_DWORD, 1)
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
