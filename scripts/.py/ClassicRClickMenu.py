import winreg
import os
import subprocess
import webbrowser


path = r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32"

print("1 - Apply this tweak.")
print("2 - Revert to default settings.")
print("3 - Docs")

x = input("")

if x == "1":
    print("Applying Windows 10 right-click menu...")
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
    winreg.SetValueEx(key, "", 0, winreg.REG_SZ, "")
    winreg.CloseKey(key)
    print("Restarting Explorer to apply changes...")
    script_dir = os.path.dirname(os.path.abspath(__file__))
    bat_file = os.path.join(script_dir, "rexplorer.bat")
    subprocess.call(bat_file)

elif x == "2":
    try:
        print("Reverting to default Windows 11 right-click menu.")
        winreg.DeleteKey(winreg.HKEY_CURRENT_USER, path)
        winreg.DeleteKey(winreg.HKEY_CURRENT_USER,r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}")
    except FileNotFoundError:
        print("Error. Windows 11 menu is already set.")
    
    print("Restarting Explorer to apply changes...")
    script_dir = os.path.dirname(os.path.abspath(__file__))
    bat_file = os.path.join(script_dir, "rexplorer.bat")
    subprocess.call(bat_file)

elif x == "3":
        webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#rmenu")
        print("Opening docs...")

else:
    print("Invalid.")
    exit()

input("")