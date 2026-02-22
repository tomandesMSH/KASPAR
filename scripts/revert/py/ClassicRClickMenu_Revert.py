import winreg
import os
import subprocess
import sys

try:
    try:
        winreg.DeleteKey(winreg.HKEY_CURRENT_USER, r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32")
        winreg.DeleteKey(winreg.HKEY_CURRENT_USER, r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}")
    except FileNotFoundError:
        pass

    script_dir = os.path.dirname(os.path.abspath(sys.executable if getattr(sys, 'frozen', False) else __file__))
    bat_file = os.path.join(script_dir, "rexplorer.bat")
    subprocess.Popen(bat_file, shell=True)

except Exception as e:
    print("Error:", e)
    sys.exit(1)
