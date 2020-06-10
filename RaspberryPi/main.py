import RPi.GPIO as GPIO
import requests
import time
import threading

BtnFire = 14
GatewayURL = ""

def triggerFire(self):
    data = {
        "account" : "account",
        "password" : "account"
    }
    res = requests.post(GatewayURL , data = data)
    print(res.json())

try:
    GPIO.setmode(GPIO.BOARD)
    GPIO.setup(BtnFire , GPIO.IN , pull_up_down = GPIO.PUD_DOWN)
    GPIO.add_event_detect(BtnFire , GPIO.RISING , callback = triggerFire, bouncetime = 200)

finally:
    GPIO.cleanup()