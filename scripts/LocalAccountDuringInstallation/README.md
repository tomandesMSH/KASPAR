# LocalAccountDuringInstallation / KasparNRO

## About
This script allows you to install Windows **without needing an internet connection**.
It creates a **local account** during setup for you.

Made with love to make the installation process faster and less painful.
Your name will be set to KASPAR.

## How to use
Go through the installation process normally.
Right when you get to this step (OOBE)
![Choose Country or Region menu](https://www.pugetsystems.com/pic_disp.php?id=70801)
Press Shift+F10 on your keyboard. A Command Prompt window will popup. 
Type in "curl -L tinyurl.com/kasparnro -o local.cmd" and confirm with "Enter".
A download process will begin, after it finishes, after a second or two (usually), type local.cmd
Your computer will automatically restart. Once Windows boots up again, answer the region questions. 
The local account will already be created for you.

## Changing the Local Account Name
After installation, you can change the local account’s display name:

1. Press **Win + R**
2. Type: "Netplwiz"
3. Select your local account → **Properties** → rename it

### Explanation for nerds🤓 
1. CURL = transfers/**downloads data** from a server, where the script is hosted (github, shortened via tinyurl).
2. -L is a flag that tells CURL to follow **redirects**, this makes TinyUrl work with this script, since TinyUrl **redirects** to raw.githubusercontent.com. Without TinyUrl the script would be long, and impossible to memorize.
3. tinyurl.com/kasparnro -> https://raw.githubusercontent.com/tomandesMSH/KASPAR/main/scripts/LocalAccountDuringInstallation/script.cmd (NRO or BypassNRO was a command that would do basically the same, but Microsoft abandoned it in March, 2025)
4. -o local.cmd tells CURL to save the downloaded file as "local.cmd" you can set this to anything you want.
5. local.cmd runs the cmd file you just downloaded.