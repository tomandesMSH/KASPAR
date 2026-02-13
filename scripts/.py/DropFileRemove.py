import winreg
import ctypes
import sys
import webbrowser

# Admin check
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()

PATH = r"SYSTEM\ControlSet001\Control\FeatureManagement\Overrides\14\3895955085"

print("1 - Apply this tweak.")
print("2 - Revert to default settings.")
print("3 - Docs")
x = input("")

if x == "1":
    key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, PATH)
    winreg.SetValueEx(key, "EnabledState", 0, winreg.REG_DWORD, 1)
    winreg.SetValueEx(key, "EnabledStateOptions", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)
    print("Tweak applied successfully.")

elif x == "2":
    try:
        key = winreg.OpenKey(
            winreg.HKEY_LOCAL_MACHINE,
            PATH,
            0,
            winreg.KEY_SET_VALUE
        )
        winreg.DeleteValue(key, "EnabledState")
        winreg.DeleteValue(key, "EnabledStateOptions")
        winreg.CloseKey(key)
        print("Tweak reverted successfully.")
    except FileNotFoundError:
        print("Nothing to revert. Values not found.")

elif x == "3":
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#dropfileremove")
    print("Opening docs...")

else:
    print("Invalid.")

input("Press Enter to exit...")
