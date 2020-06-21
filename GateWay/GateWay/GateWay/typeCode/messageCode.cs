using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWay.typeCode
{
    public class MessageCode
    {
        public enum gatewayCode
        {
            fireAlarm = 1,
            alive = 2,
            alarmResponse = 3,
            sensorAlarm = 4,
            gatewayReponse = 5,
            registerResponse = 6,
            stopAlarm = 7
        }
    }
}
