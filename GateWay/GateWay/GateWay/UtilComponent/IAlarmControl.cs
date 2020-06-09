using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    public interface IAlarmControl
    {
        public bool isAlarm();
        public void setAlarm();
        public void setSafe();
    }
}
