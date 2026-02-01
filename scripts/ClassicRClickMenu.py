import winreg
import os
import subprocess


try:
    base_path = r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32"
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, base_path)
    winreg.SetValueEx(key, "", 0, winreg.REG_SZ, "")
    winreg.CloseKey(key)
    print("Registry updated successfully! You need to restart Explorer for changes to take effect.")
except Exception as e:
    print("Error:", e)

