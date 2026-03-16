import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, " ".join(sys.argv), None, 1)
    sys.exit()

path = r"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"

try:
    key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
    try:
        winreg.DeleteValue(key, "VerboseStatus")
    except FileNotFoundError:
        pass
    winreg.CloseKey(key)
except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
