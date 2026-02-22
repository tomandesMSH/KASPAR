import winreg
import ctypes
import sys

if not ctypes.windll.shell32.IsUserAnAdmin():
    ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
    sys.exit()

try:
    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo")
    winreg.SetValueEx(key, "Enabled", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"Software\Microsoft\Windows\CurrentVersion\Privacy")
    winreg.SetValueEx(key, "TailoredExperiencesWithDiagnosticDataEnabled", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager")
    winreg.SetValueEx(key, "SubscribedContent-338393Enabled", 0, winreg.REG_DWORD, 0)
    winreg.SetValueEx(key, "SubscribedContent-353694Enabled", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

    key = winreg.CreateKey(winreg.HKEY_LOCAL_MACHINE, r"Software\Policies\Microsoft\Windows\DataCollection")
    winreg.SetValueEx(key, "AllowTelemetry", 0, winreg.REG_DWORD, 0)
    winreg.CloseKey(key)

except Exception as e:
    sys.exit(1)
