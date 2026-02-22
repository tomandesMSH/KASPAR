import winreg
import ctypes
import sys
import webbrowser
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()

print("1 - Apply this tweak")
print("2 - Revert to default settings")
print("3 - Docs")

x = input("")

path = r"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"

try:
    if x == "1":
        key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
        winreg.SetValueEx(key, "VerboseStatus", 0, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
        print("Detailed Shutdown enabled.")
        print("Tweak will work after restart.")

    elif x == "2":
        key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
        try:
            winreg.DeleteValue(key, "VerboseStatus")
            print("Detailed Shutdown disabled (set to default).")
            print("Reverting will work after restart.")
        except FileNotFoundError:
            print("Detailed Shutdown is already in default.")
        winreg.CloseKey(key)

    elif x == "3":
        webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts#detailedshutdown")
        print("Opening documentation...")

    else:
        print("Invalid.")

except PermissionError:
    print("ERROR: Permission denied. Run this script as Administrator.")
except Exception as e:
    print("Unexpected error:" , e)

input("Press Enter to exit...")