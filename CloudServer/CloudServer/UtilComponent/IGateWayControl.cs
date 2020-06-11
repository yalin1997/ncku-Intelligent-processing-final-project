using CloudServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public interface IGateWayControl
    {
        public GateWayModel findClosestGateWay(MobileDevicesModel mobile);
        public void setGateWayAlarm(string gateWayId, bool onFire);
        public bool gateWayRegister(GateWayModel gateWayInfo);
    }
}
