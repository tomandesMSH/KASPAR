@echo off
title KASPAR Launcher
echo.
echo  =========================================
echo   KASPAR - Loading, please wait...
echo  =========================================
echo.

:: ── Check if Node.js is installed ────────────────────────────────────────────
node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo  [OK] Node.js is already installed.
    goto :install
)

:: ── Node.js not found  ────────────────────────────────
echo  [..] Node.js not found. Running installer, please wait...
echo       Follow the installation steps, then come back here.
echo 	   Installing Chocolatey is not necessary.
echo.

set INSTALLER=%~dp0files\node.msi

if not exist "%INSTALLER%" (
    echo  [!!] Installer not found at: installer\node-v20.11.1-x64.msi
    echo       Please download Node.js from https://nodejs.org and install it manually.
    pause
    exit /b 1
)

msiexec /i "%INSTALLER%" /norestart
if %errorlevel% neq 0 (
    echo.
    echo  [!!] Installation failed or was cancelled.
    pause
    exit /b 1
)

:: Refresh PATH so node is available in this session
set "PATH=%PATH%;C:\Program Files\nodejs"

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
