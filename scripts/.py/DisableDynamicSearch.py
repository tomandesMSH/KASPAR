import winreg
import webbrowser

print("1 - Disable Dynamic Search")
print("2 - Revert Dynamic Search settings")
print("3 - Docs")

x = input("")

path = r"Software\Microsoft\Windows\CurrentVersion\SearchSettings"
value_name = "IsDynamicSearchBoxEnabled"

try:
    reg_key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, path, 0, winreg.KEY_SET_VALUE)
except FileNotFoundError:
    reg_key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)

if x == "1":
    winreg.SetValueEx(reg_key, value_name, 0, winreg.REG_DWORD, 0)
    print("Dynamic Search disabled sucesfully.")
elif x == "2":
    winreg.SetValueEx(reg_key, value_name, 0, winreg.REG_DWORD, 1)
    print("Dynamic Search Box settings have been reverted sucesfully.")
elif x == "3":
    webbrowser.open("tomandesmsh.github.io/KASPARDocs/scripts#disabledynamicsearch")
else:
    print("Invalid.")