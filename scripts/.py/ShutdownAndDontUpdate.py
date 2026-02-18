import winreg
import webbrowser

path = r"SOFTWARE\Microsoft\WindowsUpdate\Orchestrator"

print("Enable Shutdown and Restart options (no forced Update and Restart)")

choice = input("Select an option: ").strip()

if choice == "1":
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, path, 0,
                             winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY)

        winreg.DeleteValue(key, "EnhancedShutdownEnabled")
        winreg.CloseKey(key)

        print("Shutdown and Restart options restored!")

    except FileNotFoundError:
        print("The EnhancedShutdownEnabled value does not exist. There is no update pending..")
    except Exception as e:
        print("An error occurred: ", e)


elif choice == "2":
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, path, 0,
                             winreg.KEY_SET_VALUE | winreg.KEY_WOW64_64KEY)

        winreg.SetValueEx(key, "EnhancedShutdownEnabled", 0, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)

        print("Windows Updates enabled.")

    except Exception as e:
        print("An error occurred: ", e)


elif choice == "3":
    print("Opening docs...")
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#shutdownanddontupdate")


else:
    print("Invalid.")

input("Press Enter to exit...")