import subprocess
import os
import ctypes
import sys
import webbrowser
#admin check
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

print("1 - Create a local account")
print("2 - Docs")
x = input("")

if x == "2":
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#localaccount")
    exit()

username = input("Username: ")
password = input("Password: ")

admin = input("Make this user admin? (y/n): ").lower()


subprocess.run(
    ["net", "user", username, password, "/add"],
    check=True
)
print(username+" account was created sucesfully.")


if admin == "y":
    subprocess.run(
        ["net", "localgroup", "Administrators", username, "/add"],
        check=True
    )
    print("Adding " + username + " to the Administrators group")
    print("User created as administrator.")
else:
    print("User created as standard user.")

input("Press Enter to exit...")