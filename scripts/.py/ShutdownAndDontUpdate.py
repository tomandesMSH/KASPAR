import winreg
import webbrowser
import ctypes
import sys
# Admin Check
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

path = r"SOFTWARE\Microsoft\WindowsUpdate\Orchestrator"

print("1 - Disable Forced Windows Updates (Shutdown + Update and Shutdown)")
print("2 - Enable Forced Windows Updates (Update and Shutdown only)")
print("3 - Disable Update option entirely (Shutdown only)")
print("4 - Docs")

x = input("")

if x == "1":
    try:
        key = winreg.OpenKey(
            winreg.HKEY_LOCAL_MACHINE,
            path,
            0,
            winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY
        )

        winreg.SetValueEx(key, "ShutdownFlyoutOptions", 0, winreg.REG_DWORD, 0xF)
        winreg.CloseKey(key)

        print("only Shutdown and only Restart options are back.")

    except Exception as e:
        print("error:", e)


elif x == "2":
    try:
        key = winreg.OpenKey(
            winreg.HKEY_LOCAL_MACHINE,
            path,
            0,
            winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY
        )

        winreg.SetValueEx(key, "ShutdownFlyoutOptions", 0, winreg.REG_DWORD, 0xA)
        winreg.CloseKey(key)

        print("Forced Updates enabled.")

    except Exception as e:
        print("error:", e)


elif x == "3":
    try:
        key = winreg.OpenKey(
            winreg.HKEY_LOCAL_MACHINE,
            path,
            0,
            winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY
        )

        winreg.SetValueEx(key, "ShutdownFlyoutOptions", 0, winreg.REG_DWORD, 0x0)
        winreg.CloseKey(key)

        print("Windows Update options removed entirely.")

    except Exception as e:
        print("error:", e)


elif x == "4":
    print("Opening docs...")
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#shutdownanddontupdate")

else:
    print("Invalid.")

input("Press Enter to exit...")