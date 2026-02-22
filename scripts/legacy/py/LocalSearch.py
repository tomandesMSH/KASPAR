import winreg
import webbrowser

print("1 - Apply this tweak")
print("2 - Revert to default settings")
print("3 - Docs")

x = input("")

path = r"Software\Policies\Microsoft\Explorer"

try:
    if x == "1":
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "DisableSearchBoxSuggestions", 0, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
        print("Online features in Search box have been disabled.")
        print("The computer might require a restart to apply the changes.")

    elif x == "2":
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        try:
            winreg.DeleteValue(key, "DisableSearchBoxSuggestions")
            print("Online search in Search box restored to default.")
            print("The computer might require a restart to revert.")
        except FileNotFoundError:
            print("File not found. Tweak was already reverted before. Did you restart your computer after disabling the tweak?")
        winreg.CloseKey(key)

    elif x == "3":
        webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts#onlinesearchstartmenu")
        print("Opening documentation...")

    else:
        print("Invalid.")

except PermissionError:
    print("Permission denied.. Weird.. this script does not require admin access. You can try it again with admin priviliges, or check if this directory is managed by other programs.")
except Exception as e:
    print("error:", e)

input("Press Enter to exit...")