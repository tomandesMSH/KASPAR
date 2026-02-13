#Pynput program detecting the down arrow key. Used for Windows Country Codes

from pynput import keyboard
pressed = 0

def on_press(key):
    global pressed
    try:
        if key == keyboard.Key.down:
            pressed += 1
            print(pressed)
    except AttributeError:
        pass

with keyboard.Listener(on_press=on_press) as listener:
    listener.join()
