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
        public bool findGateWay(string gateWayId, out GateWayModel gateway);
        public void setGateWayAlarm(string gateWayId, bool onFire);
        public bool gateWayRegister(GateWayModel gateWayInfo);
        public List<GateWayModel> getGateWayList();
        public void removeGateWay(GateWayModel gateway);
    }
}
