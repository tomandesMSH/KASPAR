[![Documentation](https://img.shields.io/badge/Documentation-–?style=for-the-badge&logo=gitbook&color=blue)](https://tomandes.eu/KASPARDocs/)
![Logo](https://github.com/tomandesMSH/KASPAR/blob/main/files/kasparlogo.png?raw=true)

## About

KASPAR is a Windows utility for tweaking settings that aren't exposed in the standard UI. It lets advanced users modify registry keys and other system options safely, through a normal app instead of digging through `regedit` or Group Policy by hand.

## Features

- Modify Windows settings that are normally hidden or hard to reach
- Applies the registry changes for you
- Tweaks can be reverted

## Requirements

- Windows 10 or later
- [.NET SDK](https://dotnet.microsoft.com/download), if building from source (the app shell is [WinUI 3](https://learn.microsoft.com/windows/apps/winui/winui3/))

## Usage

Download `KASPAR-win-Setup.exe` from [Releases](https://github.com/tomandesMSH/KASPAR/releases), install it, and pick a script to run.

To run from source instead:

```
git clone https://github.com/tomandesMSH/KASPAR.git
cd KASPAR/src/KASPAR
dotnet run
```

## Documentation

Full docs: https://tomandes.eu/KASPARDocs/
