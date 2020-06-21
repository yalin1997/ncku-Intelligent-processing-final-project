using CloudServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public interface IGatewayControl
    {
        public GatewayModel findClosestGateway(MobileDevicesModel mobile);
        public bool findGateway(string gatewayId, out GatewayModel gateway);
        public void setGatewayAlarm(string gatewayId, bool onFire);
        public bool gatewayRegister(GatewayModel gatewayInfo);
        public IReadOnlyList<GatewayModel> getGatewayList();
        public void removeGateway(GatewayModel gateway);
    }
}
