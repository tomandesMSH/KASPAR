Write-Host "Creating a new restore point."
Set-ItemProperty -Path "HKLM:\Software\Microsoft\Windows NT\CurrentVersion\SystemRestore" `
    -Name "SystemRestorePointCreationFrequency" -Value 0 -Type DWord -Force

try {
    Checkpoint-Computer -Description "KASPAR Restore Point" -RestorePointType "MODIFY_SETTINGS" -ErrorAction Stop
    Write-Host "System restore point was created successfully."
} catch {
    Write-Host "Failed to create restore point: $_"
}

Set-ItemProperty -Path "HKLM:\Software\Microsoft\Windows NT\CurrentVersion\SystemRestore" `
    -Name "SystemRestorePointCreationFrequency" -Value 1440 -Type DWord -Force

pause