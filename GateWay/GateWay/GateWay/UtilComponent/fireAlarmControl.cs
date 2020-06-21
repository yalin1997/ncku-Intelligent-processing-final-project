using GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    class FireAlarmControl : IAlarmControl
    {
        public bool alarmTime = false;
        public string gatewayId { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public FireAlarmControl()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            gatewayId = "gw" + random.Next(1000).ToString();
            longitude = 120.223567f + random.Next(-1000, 1000) * 0.00001f;
            latitude = 22.997855f + random.Next(-1000, 1000) * 0.00001f;
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
            return gatewayId;
        }

        public float getLon()
        {
            return longitude;
        }

        public float getLat()
        {
            return latitude;
        }
    }
}
