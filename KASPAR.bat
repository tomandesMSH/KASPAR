@echo off
title KASPAR — Dev Launcher
echo.
echo  =========================================
echo   KASPAR - Developer Mode
echo  =========================================
echo.
echo  [i] This launcher is for developers only.
echo      End users should run the installer.
echo.

:: ── Check if Node.js is installed ────────────────────────────────────────────
node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo  [OK] Node.js found.
    goto :install
)

:: ── Node.js not found ─────────────────────────────────────────────────────────
echo  [..] Node.js not found. Running installer...
echo.

set INSTALLER=%~dp0files\node.msi

if not exist "%INSTALLER%" (
    echo  [!!] Installer not found at: files\node.msi
    echo       Download Node.js from https://nodejs.org and install manually.
    pause
    exit /b 1
)

msiexec /i "%INSTALLER%" /norestart
if %errorlevel% neq 0 (
    echo  [!!] Installation failed or was cancelled.
    pause
    exit /b 1
)

:: Refresh PATH
set "PATH=%PATH%;C:\Program Files\nodejs"
echo  [OK] Node.js installed.
echo.

:install
:: ── npm install ───────────────────────────────────────────────────────────────
echo  [..] Installing dependencies...
call npm install --silent

if %errorlevel% neq 0 (
    echo  [!!] npm install failed.
    pause
    exit /b 1
)

echo  [OK] Dependencies ready.
echo.
echo  [OK] Launching KASPAR (dev mode)...
echo.
call npm start
