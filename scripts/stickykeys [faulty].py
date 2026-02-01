import winreg

key_path = r"Control Panel\Accessibility\StickyKeys"

# Open key
key = winreg.OpenKey(
    winreg.HKEY_CURRENT_USER,
    key_path,
    0,
    winreg.KEY_READ | winreg.KEY_SET_VALUE
)

# Read current Flags
flags_str, reg_type = winreg.QueryValueEx(key, "Flags")
flags = int(flags_str)

# Ask user
choice = input("Do you want to disable the StickyKeys Shift popup? (y/n) ")

if choice.lower() == "y":
    # Disable popup → clear bit 1 (value 32)
    new_flags = flags & ~32
    winreg.SetValueEx(key, "Flags", 0, winreg.REG_SZ, str(new_flags))
    print("StickyKeys Shift popup disabled.")
else:
    # Restore original value
    winreg.SetValueEx(key, "Flags", 0, winreg.REG_SZ, str(flags))
    print("StickyKeys Shift popup restored.")

winreg.CloseKey(key)
