from pathlib import Path
import subprocess
import ctypes
import sys
import webbrowser

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()

# When compiled with PyInstaller, __file__ points to a temp folder.
# sys.executable gives us the actual .exe path, so we look for the .ps1 next to it.
if getattr(sys, 'frozen', False):
    script_dir = Path(sys.executable).resolve().parent
else:
    script_dir = Path(__file__).resolve().parent

print("1 - Create a restore point")
print("2 - Delete all restore points")
print("3 - Docs")
x = input("")

if x == "1":
    powershell_script = script_dir / "restorepointhelper.ps1"
    subprocess.run([
        "powershell",
        "-ExecutionPolicy", "Bypass",
        "-File", str(powershell_script)
    ])

if x == "2":
    command = ["vssadmin", "delete", "shadows", "/for=c:", "/all", "/quiet"]
    try:
        subprocess.run(command, check=True)
        print("All restore points deleted successfully.")
    except subprocess.CalledProcessError as e:
        print("Error:", e)
        print("(Error - No items found that satisfy the query = you have no restore points.)")
    input("")

if x == "3":
    print("Opening docs...")
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts#restorepoint")
