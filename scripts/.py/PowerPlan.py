import os
import webbrowser
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()

print("1 - Create a new power plan")
print("2 - Remove a power plan")
print("3 - Docs")

x = input("")

power_saving = "a1841308-3541-4fab-bc81-f71556f20b4a"
balanced = "381b4222-f694-41f0-9685-ff5bb260df2e"
maximum = "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"
ultimate = "e9a42b02-d5df-448d-aa00-03f14749eb61"

if x == "1":
    print("Choose base power plan:")
    print("1 - Power saving")
    print("2 - Balanced")
    print("3 - Maximum performance")
    print("4 - Ultra performance")

    p = input("")

    if p == "1":
        base_plan = power_saving
        base_name = "Power Saving plan"
    elif p == "2":
        base_plan = balanced
        base_name = "Balanced plan"
    elif p == "3":
        base_plan = maximum
        base_name = "High-performance plan"
    elif p == "4":
        base_plan = ultimate
        base_name = "Ultimate Performance plans"
    else:
        print("Invalid choice.")
        exit()

    name = input("Make a custom name for your power plan: ")

    os.system("powercfg -duplicatescheme " + base_plan)

    output = os.popen("powercfg -list").read().splitlines()

    new = ""
    for line in output:
        if base_name in line:
            new = line.split(":")[1].split("(")[0].strip()

    if new != "":
        os.system('powercfg -changename ' + new + ' "' + name + '"')
        os.system("powercfg -setactive " + new)
        print("The " + name + " power plan was created successfully.")
    else:
        exit()

elif x == "2":
    print("Choose a plan to remove:")
    print("The plan with the asterisk(*) is currently in use, and cannot be deleted unless you change the active plan.")
    os.system("powercfg -list")

    guid = input("Enter the GUID of the plan you would like to delete: ")
    confirm = input("Are you sure? (y/n): ")

    if confirm.lower() == "y":
        os.system("powercfg -delete " + guid)
        print("Done.")
    else:
        print("Action cancelled.")

elif x == "3":
    print("Opening docs...")
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#powerplan")

else:
    print("Incorrect.")

input("Press Enter to exit...")