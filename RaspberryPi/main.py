import RPi.GPIO as GPIO
import requests
import time
import threading
import sys

BtnFire = 12
StateLED = 11
BtnStop = 10
AlarmLED = 8
GatewayURL = "http://172.24.54.1:5002/api/"

def triggerFire(self):
    print("Trigger fire.")
    data = {
        "account" : "account",
        "password" : "account"
    }
    headers={'Content-type':'application/json', 'Accept':'application/json'}
    res = requests.post(GatewayURL+'fireAlarm/getFireAlarm' ,headers=headers , json = data , timeout = 1)
    if(res.status_code == 200):
        print(res.json())
    else:
        print(res)

def triggerStop(self):
    print("Trigger fire.")
    data = {
        "account" : "account",
        "password" : "account"
    }
    headers={'Content-type':'application/json', 'Accept':'application/json'}
    res = requests.post(GatewayURL+'fireAlarm/fireAlarmStop' ,headers=headers , json = data , timeout = 1)
    if(res.status_code == 200):
        print(res.json())
    else:
        print(res)

class ControlLEDTrhead (threading.Thread):
    def __init__(self , led_pin):
        threading.Thread.__init__(self)
        self._interval = 1
        self._active = False
        self._isStop = False
        self._light = False
        self._counter = 0
        self._led_pin = led_pin
                
    def run(self):
        while(not self._isStop):
            if(self._active):
                if(self._interval < 0):
                    GPIO.output(self._led_pin , True)
                else:
                    self._counter = self._counter % self._interval
                    if(self._counter == 0):
                        self._light = not self._light
                        GPIO.output(self._led_pin , self._light)
                    self._counter += 1
            else:
                GPIO.output(self._led_pin , False)
                
            time.sleep(1)

    def setState(self , active , interval):
        self._interval = interval
        self._active = active
        self._counter = 0
        self._light = False
    
    def stop(self):
        self._isStop = True

class UpdateInformation (threading.Thread):
    def __init__(self , ledControl):
        threading.Thread.__init__(self)
        self._isStop = False
        self._ledControl = ledControl

    def run(self):
        last = False
        while(not self._isStop):
            data = {
                "account" : "account",
                "password" : "account"
            }
            headers={'Content-type':'application/json', 'Accept':'application/json'}
            res = requests.post(GatewayURL+'fireAlarm/SensorAlarm' ,headers=headers , json = data , timeout = 1)
            if(res.status_code == 200):
                print(res.json())
                if(res.json()["content"] == 'true' and last == False):
                    self._ledControl.setState(True , 1)
                    last = True
                else:
                    self._ledControl.setState(False , 1)
                    last = False
            else:
                print(res)
            time.sleep(1)

    def stop(self):
        self._isStop = True

try:
    GPIO.setmode(GPIO.BOARD)
    GPIO.setup(BtnFire , GPIO.IN , pull_up_down = GPIO.PUD_DOWN)
    GPIO.setup(BtnStop , GPIO.IN , pull_up_down = GPIO.PUD_DOWN)
    GPIO.setup(StateLED , GPIO.OUT)
    GPIO.setup(AlarmLED , GPIO.OUT)
    GPIO.add_event_detect(BtnFire , GPIO.RISING , callback = triggerFire, bouncetime = 200)
    GPIO.add_event_detect(BtnStop , GPIO.RISING , callback = triggerStop, bouncetime = 200)

    LEDControlState = ControlLEDTrhead(StateLED)
    LEDControlAlarm = ControlLEDTrhead(AlarmLED)
    UpdateInformation = UpdateInformation(LEDControlAlarm)
    LEDControlState.start()
    UpdateInformation.start()
    LEDControlAlarm.start()

    LEDControlState.setState(True , 2)

    while(True):
        time.sleep(1)

except:
    a, b, c = sys.exc_info()
    print(a)
    print(b)
    print(c)

    if(LEDControlState.isAlive()):
        LEDControlState.stop()
        LEDControlState.join()

    if(LEDControlAlarm.isAlive()):
        LEDControlAlarm.stop()
        LEDControlAlarm.join()

    if(UpdateInformation.isAlive()):
        UpdateInformation.stop()
        UpdateInformation.join()

finally:
    GPIO.cleanup()