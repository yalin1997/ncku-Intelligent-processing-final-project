using CloudServer.Code;
using CloudServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer.UtilComponent
{
    public class GateWayControl : IGateWayControl
    {
        private Dictionary<string, GateWayModel> GatewayDictionary = new Dictionary<string, GateWayModel>();
        public IReadOnlyList<GateWayModel> GateWayList { get => GatewayDictionary.Values.ToList(); }
        public GateWayControl() { }
        private double alarmThreshold = 10;
        private readonly IHttpClientFactory _clientFactory;
        
        public GateWayControl(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public static double calculateDistance(double x1, double y1, double x2, double y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        public GateWayModel findClosestGateWay(MobileDevicesModel mobile)
        {
            double closest =9999.9;
            GateWayModel closestGateWay = GateWayList[0];
            foreach (GateWayModel item in GateWayList)
            {
                if (!item.isActive)
                {
                    continue;
                }
                double value = calculateDistance(item.longitude, item.latitude, mobile.longitude, mobile.latitude);
                if (value < closest)
                {
                    closest = value;
                    closestGateWay = item;
                }
            }
            return closestGateWay;
        }
        public GateWayModel findClosestGateWay(GateWayModel gateway)
        {
            double closest = 9999.9;
            GateWayModel closestGateWay = GateWayList[0];
            foreach (GateWayModel item in GateWayList)
            {
                if (item.latitude == gateway.latitude && item.longitude == gateway.longitude)
                {
                    continue;
                }
                double value = calculateDistance(item.longitude, item.latitude, gateway.longitude, gateway.latitude);
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
            if (GatewayDictionary.ContainsKey(gateWayId))
            {
                gateway = GatewayDictionary[gateWayId];
                return true;
            }
            gateway = new GateWayModel { gateWayId = "-1" };
            return false;
        }
        public void setClosestGateWayAlarm(GateWayModel  gateway)
        {
            foreach (GateWayModel item in GateWayList)
            {
                if (calculateDistance(gateway.latitude , item.latitude , gateway.longitude , item.longitude) <= alarmThreshold)
                {
                    item.isAlarm = true;
                    bool result;
                    do
                    {
                        result = SendToOtherGateWay(gateway.gateWayUri, gateway.gateWayId, DateTime.Now);
                    } while (result == false);
                }
            }
        }
        public void setGateWayAlarm(string gateWayId, bool onFire)
        {
            throw new NotImplementedException();
        }
        public bool gateWayRegister(GateWayModel gateWayInfo)
        {
            if(GatewayDictionary.ContainsKey(gateWayInfo.gateWayId))
            {
                GatewayDictionary[gateWayInfo.gateWayId].UpdateTime = DateTime.Now;
                return GatewayDictionary[gateWayInfo.gateWayId].isAlarm;
            }
            else
            {
                GatewayDictionary[gateWayInfo.gateWayId] = gateWayInfo;
                return false;
            }
        }
        private bool SendToOtherGateWay(string url, string gateWayId, DateTime time  )
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            GateWayMessageModel postData = new GateWayMessageModel() { gateWayId = gateWayId ,  messageType = (int)messageCode.gateWayCode.fireAlarm , messageTime = time , content = "附近的網關 " + gateWayId + " 號檢測到火災!" };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(url, contentPost).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GateWayMessageModel responseModel = JsonConvert.DeserializeObject<GateWayMessageModel>(cloudResponse);
            if (responseModel.content == "true")
            {
                return true;
            }
            return false;
        }

        public IReadOnlyList<GateWayModel> getGateWayList()
        {
            return GateWayList;
        }

        public void removeGateWay(GateWayModel gateway)
        {
            GatewayDictionary.Remove(gateway.gateWayId);
        }
    }
}
