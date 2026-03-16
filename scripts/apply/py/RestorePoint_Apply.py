from pathlib import Path
import subprocess
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, " ".join(sys.argv), None, 1)
    sys.exit()

script_dir = Path(sys.executable).resolve().parent if getattr(sys, 'frozen', False) else Path(__file__).resolve().parent
powershell_script = script_dir / "restorepointhelper.ps1"

try:
    subprocess.run([
        "powershell",
        "-ExecutionPolicy", "Bypass",
        "-File", str(powershell_script)
    ], check=True, timeout=60)
except subprocess.TimeoutExpired:
    print("ERROR: Restore point creation timed out.")
    sys.exit(1)
except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
