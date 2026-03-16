import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, " ".join(sys.argv), None, 1)
    sys.exit()

path = r"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked"

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
    winreg.SetValueEx(key, "{CB3B0003-8088-4EDE-8769-8B354AB2FF8C}", 0, winreg.REG_SZ, "")
    winreg.SetValueEx(key, "{241F3F77-3B9E-4C64-8E2E-1A7C62C2F1AE}", 0, winreg.REG_SZ, "")
    winreg.CloseKey(key)
except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
