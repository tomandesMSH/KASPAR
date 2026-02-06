print("loading...")

import pandas as pd
import winreg
import openpyxl
import os
import webbrowser

def cls():
    os.system('cls' if os.name=='nt' else 'clear')

cls()

print("1 - Change device setup region")
print("2 - See what each country restricts")
print("3 - Docs")
print("4 - Open country list")

x = input("")

if x == "1":

    script_dir = os.path.dirname(os.path.abspath(__file__))
    file = os.path.join(script_dir, "countries.xlsx")

    data = pd.read_excel(file, header=None)

    country_input = input("Enter country name: ")

    found = False

    for i in range(len(data)):
        country = str(data.iloc[i, 0])

        if country.lower() == country_input.lower():
            abbrev = str(data.iloc[i, 1])
            nation_id = int(data.iloc[i, 2])

            print("Changing current country to:", country, abbrev, nation_id)

            key = winreg.OpenKey(
                winreg.HKEY_CURRENT_USER,
                r"Control Panel\International\Geo",
                0,
                winreg.KEY_SET_VALUE
            )

            winreg.SetValueEx(key, "Name", 0, winreg.REG_SZ, abbrev)
            winreg.SetValueEx(key, "Nation", 0, winreg.REG_SZ, str(nation_id))

            winreg.CloseKey(key)

            print("User region changed.")

            try:
                key2 = winreg.OpenKey(
                    winreg.HKEY_LOCAL_MACHINE,
                    r"SOFTWARE\Microsoft\Windows\CurrentVersion\Control Panel\DeviceRegion",
                    0,
                    winreg.KEY_SET_VALUE
                )

                winreg.DeleteValue(key2, "DeviceRegion")
                winreg.CloseKey(key2)

                print("Windows setup region changed sucesfully.")

            except PermissionError:
                print("Current Region changed sucesfully. To change the Device setup region, run the file as admin.")

            except FileNotFoundError:
                print("DeviceRegion value not found, weird...")

            print("Done.")
            found = True
            break

    if found == False:
        print('Country not in list, chooose "Open country list" in the selection.')

elif x == "2":
    os.startfile(r"C:\Windows\System32\IntegratedServicesRegionPolicySet.json")

elif x == "3":
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#dropfileremove")
    print("Opening docs...")


elif x == "4":
    script_dir = os.path.dirname(os.path.abspath(__file__))
    file = os.path.join(script_dir, "countrylist.txt")
    os.startfile(file)
    print('Copy and paste the country name, after selecting "Change device setup region"')

else:
    print("Invalid")

input("Press Enter to exit...")