@echo off
echo 1 - Create the GodMode folder
echo 2 - Docs
set /p x=

for /f "tokens=2*" %%A in ('reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v Desktop') do set desktop=%%B
call set desktop=%desktop%

if "%x%"=="1" (
    powershell -Command "New-Item -Path '%desktop%\GodMode.{ED7BA470-8E54-465E-825C-99712043E01C}' -ItemType Directory -Force"
    echo GodMode folder created on Desktop!
)

if "%x%"=="2" start "" "https://tomandesmsh.github.io/KASPARDocs/scripts#godmode"

echo Press Enter to exit...
pause >nul

::This code was AI assisted, i wrote my own code but for some reason %USERPROFILE% was misbehaving and i had to use PowerShell which i have yet to learn...