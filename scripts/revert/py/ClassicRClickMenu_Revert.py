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

    try:
        subprocess.run(["cmd.exe", "/c", bat_file], timeout=15, capture_output=True)
    except subprocess.TimeoutExpired:
        print("ERROR: rexplorer.bat timed out.")
        sys.exit(1)

except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
