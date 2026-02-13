#Do not launch in VSCode. Launch with Python directly. (Admin Check breaks VSCode.)
import os
import subprocess
import time
import ctypes
import sys
import webbrowser

# Admin check
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()

print("Welcome to KASPAR.")


script_dir = os.path.dirname(os.path.abspath(__file__))

files = {
    "1": "ClassicRClickMenu.exe",
    "2": "ControlPanelGodMode.bat",
    "3": "DetailedShutdown.exe",
    "4": "DeviceSetupRegion.exe",
    "5": "DisableDynamicSearch.exe",
    "6": "DropFileRemove.exe",
    "7": "EndTask.exe",
    "8": "LocalAccount.exe",
    "9": "LocalSearch.exe",
    "10": "PowerPlan.exe",
    "11": "RestorePoint.exe",
    "12": "StartAds.exe",
    "13": "DisableAdvertisementlD.py",
}

# Build full paths
for key in files:
    files[key] = os.path.join(script_dir, files[key])

print("Select a tweak to apply:")
print("note: each tweak has links to their documentation.")
print("All documentation is available on: https://tomandesmsh.github.io/KASPARDocs/")
print(" ")
print(" ")
for key, name in files.items():
    print(f"{key}. {os.path.basename(name)}")

choice = input("Enter the number of the script you want to run (Confirm with Enter): ").strip()

if choice in ["1", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"]:
    subprocess.Popen(files[choice])

if choice == "2":
    subprocess.Popen(["cmd.exe", "/c", files["2"]])

if choice == "13":
    subprocess.Popen(["python", files["13"]])