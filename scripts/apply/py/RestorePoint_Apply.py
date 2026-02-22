from pathlib import Path
import subprocess
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

script_dir = Path(sys.executable).resolve().parent if getattr(sys, 'frozen', False) else Path(__file__).resolve().parent
powershell_script = script_dir / "restorepointhelper.ps1"

try:
    subprocess.run([
        "powershell",
        "-ExecutionPolicy", "Bypass",
        "-File", str(powershell_script)
    ], check=True)

except Exception as e:
    sys.exit(1)
