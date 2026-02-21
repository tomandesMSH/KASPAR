@echo off
title KASPAR Launcher
echo.
echo  =========================================
echo   KASPAR - Starting up...
echo  =========================================
echo.

:: ── Check if Node.js is installed ────────────────────────────────────────────
node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo  [OK] Node.js is already installed.
    goto :install
)

:: ── Node.js not found - download and install ─────────────────────────────────
echo  [..] Node.js not found. Downloading installer...
echo       This may take a minute, please wait.
echo.

curl -L "https://nodejs.org/dist/v20.11.1/node-v20.11.1-x64.msi" -o "%TEMP%\node_installer.msi"

if %errorlevel% neq 0 (
    echo.
    echo  [!!] Download failed. Check your internet connection and try again.
    pause
    exit /b 1
)

echo  [..] Installing Node.js silently...
msiexec /i "%TEMP%\node_installer.msi" /qn /norestart

:: Refresh PATH so node is available in this session
for /f "tokens=*" %%i in ('where node 2^>nul') do set NODE_PATH=%%i
if not defined NODE_PATH (
    set "PATH=%PATH%;C:\Program Files\nodejs"
)

echo  [OK] Node.js installed successfully.
echo.

:install
:: ── Run npm install ───────────────────────────────────────────────────────────
echo  [..] Installing app dependencies...
call npm install --silent

if %errorlevel% neq 0 (
    echo.
    echo  [!!] npm install failed. Try running this file again.
    pause
    exit /b 1
)

echo  [OK] Dependencies ready.
echo.

:: ── Launch the app ────────────────────────────────────────────────────────────
echo  [OK] Launching KASPAR...
echo.
call npm start
