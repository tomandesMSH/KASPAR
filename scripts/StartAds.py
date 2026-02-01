import ctypes
import sys
import webbrowser
import winreg

# Admin Check
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

print("1 - Apply this tweak.")
print("2 - Revert to default settings.")
print("3 - Docs")

x = input("Choose an option (1/2/3): ")

key_path = r"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"

try:
    key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_SET_VALUE)

    if x == "1":
        winreg.SetValueEx(key, "SystemPaneSuggestionsEnabled", 0, winreg.REG_DWORD, 0)
        print("Start menu suggestions/ads disabled!")

    elif x == "2":
        winreg.SetValueEx(key, "SystemPaneSuggestionsEnabled", 0, winreg.REG_DWORD, 1)
        print("Start menu suggestions/ads reverted to default!")

    elif x == "3":
        webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#startads")
        print("Opening docs...")

    else:
        print("Invalid.")

    winreg.CloseKey(key)

except FileNotFoundError:
    print("Registry path not found. Your Windows version might be different.")
except Exception as e:
    print(f"An error occurred: {e}")
