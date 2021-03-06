import RPi.GPIO as GPIO
import requests
import time
import threading
import sys

Buzzer = 13
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
    try:
        headers={'Content-type':'application/json', 'Accept':'application/json'}
        res = requests.post(GatewayURL+'fireAlarm/getFireAlarm' ,headers=headers , json = data , timeout = 1)
        if(res.status_code == 200):
            print(res.json())
        else:
            print(res)
    except:
        print("net work error.")

def triggerStop(self):
    print("Trigger fire.")
    data = {
        "account" : "account",
        "password" : "account"
    }
    headers={'Content-type':'application/json', 'Accept':'application/json'}
    try:
        res = requests.post(GatewayURL+'fireAlarm/fireAlarmStop' ,headers=headers , json = data , timeout = 1)
        if(res.status_code == 200):
            print(res.json())
        else:
            print(res)
    except:
        print("net work error.")


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

class BuzzerTrhead (threading.Thread):
    def __init__(self , buzzer_pin):
        threading.Thread.__init__(self)
        self._interval = 1
        self._active = False
        self._isStop = False
        self._step = 1
        self._counter = 0
        self.buzzer = GPIO.PWM(buzzer_pin, 50)
        self.buzzer.start(0)
                
    def run(self):
        while(not self._isStop):
            if(self._active):
                if(self._counter >= (880 - 523)):
                    self._step = -1
                elif(self._counter <= 0):
                    self._step = 1
                self._counter += self._step
                self.buzzer.ChangeFrequency(523 + self._counter)
            time.sleep(0.01)
        self.buzzer.stop()

    def setState(self , active ):
        if((self._active == False) and (active == True)):
            self.buzzer.ChangeDutyCycle(50)
            self._counter = 0
            self._step = 1
        elif((self._active == True) and (active == False)):
            self.buzzer.ChangeDutyCycle(0)
        self._active = active
    
    def stop(self):
        self._isStop = True

class UpdateInformation (threading.Thread):
    def __init__(self , ledControl , buzzerControl):
        threading.Thread.__init__(self)
        self._isStop = False
        self._ledControl = ledControl
        self._buzzerControl = buzzerControl

    def run(self):
        last = False
        while(not self._isStop):
            data = {
                "account" : "account",
                "password" : "account"
            }
            headers={'Content-type':'application/json', 'Accept':'application/json'}
            try:
                res = requests.post(GatewayURL+'fireAlarm/SensorAlarm' ,headers=headers , json = data , timeout = 1)
                if(res.status_code == 200):
                    print(res.json())
                    if(res.json()["content"] == 'true' and last == False):
                        self._ledControl.setState(True, 1)
                        self._buzzerControl.setState(True)
                        last = True
                    elif(res.json()["content"] == 'false' and last == True):
                        self._ledControl.setState(False , 1)
                        self._buzzerControl.setState(False)
                        last = False
                else:
                    print(res)
            except:
                print("net work error.")
            
            time.sleep(1)

    def stop(self):
        self._isStop = True

try:
    GPIO.setmode(GPIO.BOARD)
    GPIO.setup(BtnFire , GPIO.IN , pull_up_down = GPIO.PUD_DOWN)
    GPIO.setup(BtnStop , GPIO.IN , pull_up_down = GPIO.PUD_DOWN)
    GPIO.setup(StateLED , GPIO.OUT)
    GPIO.setup(AlarmLED , GPIO.OUT)
    GPIO.setup(Buzzer , GPIO.OUT)
    GPIO.add_event_detect(BtnFire , GPIO.RISING , callback = triggerFire, bouncetime = 200)
    GPIO.add_event_detect(BtnStop , GPIO.RISING , callback = triggerStop, bouncetime = 200)

    LEDControlState = ControlLEDTrhead(StateLED)
    LEDControlAlarm = ControlLEDTrhead(AlarmLED)
    BuzzerControl = BuzzerTrhead(Buzzer)
    UpdateInformation = UpdateInformation(LEDControlAlarm , BuzzerControl)
    LEDControlState.start()
    UpdateInformation.start()
    LEDControlAlarm.start()
    BuzzerControl.start()

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

    if(BuzzerControl.isAlive()):
        BuzzerControl.stop()
        BuzzerControl.join()

finally:
    GPIO.cleanup()