import winreg
import webbrowser


PATH = r"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings"

print("1 - Apply this tweak.")
print("2 - Revert to default settings.")
print("3 - Docs")

x = input("")

if x == "1":
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, PATH)
    winreg.SetValueEx(key, "TaskbarEndTask", 0, winreg.REG_DWORD, 1)
    winreg.CloseKey(key)

    print("Taskbar End Task enabled.")

elif x == "2":
    try:
        key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, PATH, 0, winreg.KEY_SET_VALUE)
        winreg.DeleteValue(key, "TaskbarEndTask")
        winreg.CloseKey(key)

        print("Taskbar End Task reverted to default.")
    except FileNotFoundError:
        print("Error. Tweak was never applied.")

elif x == "3":
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#endtask")
    print("Opening docs")

else:
    print("Incorrect.")

input("Press Enter to exit...")