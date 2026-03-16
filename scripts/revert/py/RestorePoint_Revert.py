import subprocess
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, " ".join(sys.argv), None, 1)
    sys.exit()

try:
    subprocess.run(
        ["vssadmin", "delete", "shadows", "/for=c:", "/all", "/quiet"],
        check=True,
        timeout=60
    )
except subprocess.TimeoutExpired:
    print("ERROR: vssadmin timed out.")
    sys.exit(1)
except subprocess.CalledProcessError:
    pass  # "No items found" is non-zero but not a real error
except Exception as e:
    print("ERROR:", e)
    sys.exit(1)
