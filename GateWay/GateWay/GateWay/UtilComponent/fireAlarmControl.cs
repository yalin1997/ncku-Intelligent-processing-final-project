using GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    class fireAlarmControl : IAlarmControl
    {
        public bool alarmTime = false;
        public string gateWayId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public fireAlarmControl()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            gateWayId = "gw" + random.Next(1000).ToString();
            longitude = 22.995928 + random.Next(-1, 1);
            latitude = 120.217433 + random.Next(-1, 1);
        }
        public bool isAlarm()
        {
            return alarmTime;
        }
        public void setAlarm()
        {
            alarmTime =true;
        }
        public void setSafe()
        {
            alarmTime = false;
        }

        public string getId()
        {
            return gateWayId;
        }

        public double getLon()
        {
            return longitude;
        }

        public double getLat()
        {
            return latitude;
        }
    }
}
