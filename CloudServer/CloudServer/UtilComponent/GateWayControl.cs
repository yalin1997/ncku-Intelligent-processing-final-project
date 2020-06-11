﻿using CloudServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public class GateWayControl : IGateWayControl
    {
        List<GateWayModel> GateWayList = new List<GateWayModel>();
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

        public void setGateWayAlarm(string gateWayId, bool onFire)
        {
            throw new NotImplementedException();
        }
        public bool gateWayRegister(GateWayModel gateWayInfo)
        {
            GateWayList.Add(gateWayInfo);
            return true;
        }

    }
}