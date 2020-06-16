using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWay.typeCode
{
    public class messageCode
    {
        public enum gateWayCode
        {
            fireAlarm = 1,
            alive = 2,
            alarmResponse = 3,
            sensorAlarm = 4,
            gateWayReponse = 5,
            registerResponse = 6,
            stopAlarm = 7
        }
    }
}
