import winreg
import ctypes
import sys
import webbrowser
import time
if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, __file__, None, 1
    )
    sys.exit()


print("1 - Apply this tweak.")
print("2 - Revert to default settings.")
print("3 - Docs")

x = input("")

try:
    if x == "1":

        #Ad ID
        path = r"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "Enabled", 0, winreg.REG_DWORD, 0)
        winreg.CloseKey(key)
        print("Advertising ID disabled.")
        time.sleep(0.2)
        # Tailored Experiences (formerly Personalized Offers)
        path = r"Software\Microsoft\Windows\CurrentVersion\Privacy"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "TailoredExperiencesWithDiagnosticDataEnabled", 0, winreg.REG_DWORD, 0)
        winreg.CloseKey(key)
        print("Tailored experiences disabled.")
        time.sleep(0.2)
        # Ad personalization
        path = r"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "SubscribedContent-338393Enabled", 0, winreg.REG_DWORD, 0)
        winreg.SetValueEx(key, "SubscribedContent-353694Enabled", 0, winreg.REG_DWORD, 0)
        winreg.CloseKey(key)
        print("Ad personalization disabled.")
        time.sleep(0.2)
        # Telemetry
        path = r"Software\Policies\Microsoft\Windows\DataCollection"
        key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
        winreg.SetValueEx(key, "AllowTelemetry", 0, winreg.REG_DWORD, 0)
        winreg.CloseKey(key)
        print("telemetry disabled")
        print("The computer might require a restart to apply the changes.")
        print("The Settings app might not display the correct value. That is not a problem and doesn't cause any harm. Registry keys have been changed.")

    if x == "2":

        #Ad ID
        path = r"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "Enabled", 1, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
        print("Advertising ID status restored")
        time.sleep(0.2)
        #T. Experiences
        path = r"Software\Microsoft\Windows\CurrentVersion\Privacy"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "TailoredExperiencesWithDiagnosticDataEnabled", 1, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
        print("Tailored experiences status restored")
        time.sleep(0.2)
        # Ad personalization
        path = r"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"
        key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, path)
        winreg.SetValueEx(key, "SubscribedContent-338393Enabled", 1, winreg.REG_DWORD, 1)
        winreg.SetValueEx(key, "SubscribedContent-353694Enabled", 1, winreg.REG_DWORD, 1)
        winreg.CloseKey(key)
        print("Ad personalization status restored")
        time.sleep(0.2)
        # telemetry
        path = r"Software\Policies\Microsoft\Windows\DataCollection"
        key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, path)
        winreg.SetValueEx(key, "AllowTelemetry", 3, winreg.REG_DWORD, 3)
        winreg.CloseKey(key)
        print("Telemetry status restored")
        print("The computer might require a restart to apply the changes.")
    if x == "3":
        webbrowser.open("https://tomandesmsh.github.io/KASPARDocs/scripts#disableadvertisementid")


except Exception as e:
    print("Ignore the message above me. Error found:", e)

input("Press ENTER to exit.")