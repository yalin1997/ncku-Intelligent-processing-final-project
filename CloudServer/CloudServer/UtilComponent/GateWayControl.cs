using CloudServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public class GateWayControl : IGateWayControl
    {
        public List<GateWayModel> GateWayList = new List<GateWayModel>();
        public GateWayControl() { }
        public static double calculateDistance(double x1, double y1, double x2, double y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        public GateWayModel findClosestGateWay(MobileDevicesModel mobile)
        {
            double closest =9999.9;
            GateWayModel closestGateWay = GateWayList[0];
            foreach(GateWayModel item in GateWayList)
            {
                double value = calculateDistance(item.longitude, item.latitude, mobile.longitude, mobile.latitude);
                if (value < closest)
                {
                    closest = value;
                    closestGateWay = item;
                }
            }
            return closestGateWay;
        }
        public bool  findGateWay(string gateWayId , out GateWayModel gateway)
        {
            foreach (GateWayModel item in GateWayList)
            {
                if (item.gateWayId.Equals(gateWayId))
                {
                    gateway = item;
                    item.isAlarm = true;
                    return true;
                }
            }
            gateway = new GateWayModel { gateWayId = "-1" };
            return false;
        }
        public void setGateWayAlarm(string gateWayId, bool onFire)
        {
            throw new NotImplementedException();
        }
        public bool gateWayRegister(GateWayModel gateWayInfo)
        {
            GateWayList.Add(gateWayInfo);
            return true;
        }

        public List<GateWayModel> getGateWayList()
        {
            return GateWayList;
        }

        public void removeGateWay(GateWayModel gateway)
        {
            GateWayList.Remove(gateway);
        }
    }
}
