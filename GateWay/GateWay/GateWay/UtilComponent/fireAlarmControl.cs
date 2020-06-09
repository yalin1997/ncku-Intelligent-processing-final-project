using GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    class fireAlarmControl : IAlarmControl
    {
        private bool alarmTime = false;
        private fireAlarmModel Alarm;
        public bool isAlarm()
        {
            return alarmTime;
        }
        public void handleAlarm(fireAlarmModel alarm)
        {
            this.Alarm = alarm;
        }
        public void setAlarm()
        {
            alarmTime =true;
        }
        public void setSafe()
        {
            alarmTime = false;
        }
    }
}
