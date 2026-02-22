import subprocess
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

try:
    subprocess.run(
        ["vssadmin", "delete", "shadows", "/for=c:", "/all", "/quiet"],
        check=True
    )

except subprocess.CalledProcessError:
    # "No items found" is a non-zero exit but not a real error
    sys.exit(0)
except Exception as e:
    sys.exit(1)
