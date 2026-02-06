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
    file = os.path.join(script_dir, "WindowsCountryListFixed.xlsx")

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
    webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts.html#devicesetupregion")
    print("Opening docs...")


elif x == "4":
    print('''Afghanistan
Åland Islands
Albania
Algeria
American Samoa
Andorra
Angola
Anguilla
Antarctica
Antigua and Barbuda
Argentina
Armenia
Aruba
Australia
Austria
Azerbaijan
Bahamas, The
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bermuda
Bhutan
Bolivia
Bonaire, Sint Eustatius and Saba
Bosnia and Herzegovina
Botswana
Bouvet Island
Brazil
British Indian Ocean Territory
British Virgin Islands
Brunei
Bulgaria
Burkina Faso
Burundi
Cabo Verde
Cambodia
Cameroon
Canada
Cayman Islands
Central African Republic
Chad
Chile
China
Christmas Island
Cocos (Keeling) Islands
Colombia
Comoros
Congo
Congo (DRC)
Cook Islands
Costa Rica
Côte d'Ivoire
Cuba
Croatia
Curaçao
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Eswatini
Ethiopia
Falkland Islands
Faroe Islands
Fiji
Finland
France
French Guiana
French Polynesia
French Southern Territories
Gabon
Gambia
Georgia
Germany
Ghana
Gibraltar
Greece
Greenland
Grenada
Guadeloupe
Guam
Guatemala
Guernsey
Guinea
Guinea-Bissau
Guyana
Haiti
Heard Island and McDonald Islands
Honduras
Hong Kong SAR
Hungary
Iceland
India
Indonesia
Iran
Iraq
Ireland
Isle of Man
Israel
Italy
Jamaica
Japan
Jersey
Jordan
Kazakhstan
Kenya
Kiribati
Korea
Kosovo
Kuwait
Kyrgyzstan
Laos
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macao SAR
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Martinique
Mauritania
Mauritius
Mayotte
Mexico
Micronesia
Moldova
Monaco
Mongolia
Montenegro
Montserrat
Morocco
Mozambique
Myanmar
Namibia
Nauru
Nepal
Netherlands
New Caledonia
New Zealand
Nicaragua
Niger
Nigeria
Niue
Norfolk Island
North Korea
North Macedonia
Northern Mariana Islands
Norway
Oman
Pakistan
Palau
Palestinian Authority
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Pitcairn Islands
Poland
Portugal
Puerto Rico
Qatar
Réunion
Romania
Russia
Rwanda
Saint Barthélemy
Saint Kitts and Nevis
Saint Lucia
Saint Martin
Saint Pierre and Miquelon
Saint Vincent and the Grenadines
Samoa
San Marino
São Tomé and Príncipe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Sint Maarten
Slovakia
Slovenia
Solomon Islands
Somalia
South Africa
South Georgia and the South Sandwich Islands
South Sudan
Spain
Sri Lanka
St Helena, Ascension and Tristan da Cunha
Sudan
Suriname
Svalbard
Sweden
Switzerland
Syria
Taiwan
Tajikistan
Tanzania
Thailand
Timor-Leste
Togo
Tokelau
Tonga
Trinidad and Tobago
Tunisia
Türkiye
Turkmenistan
Turks and Caicos Islands
Tuvalu
U.S. Minor Outlying Islands
U.S. Virgin Islands
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
Uruguay
Uzbekistan
Vanuatu
Vatican City
Venezuela
Vietnam
Wallis and Futuna
Yemen
Zambia
Zimbabwe
''')
    print('Copy and paste the country name, after selecting "Change device setup region"')

else:
    print("Invalid")

input("Press Enter to exit...")