import winreg
import os
import subprocess
import sys

path = r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32"

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
    winreg.SetValueEx(key, "", 0, winreg.REG_SZ, "")
    winreg.CloseKey(key)

    script_dir = os.path.dirname(os.path.abspath(sys.executable if getattr(sys, 'frozen', False) else __file__))
    bat_file = os.path.join(script_dir, "rexplorer.bat")
    subprocess.call(bat_file)

except Exception as e:
    print("Error:", e)
    input("Press Enter to exit...")
    sys.exit(1)
