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
    public class GatewayControl : IGatewayControl
    {
        private Dictionary<string, GatewayModel> GatewayDictionary = new Dictionary<string, GatewayModel>();
        public IReadOnlyList<GatewayModel> GatewayList { get => GatewayDictionary.Values.ToList(); }
        public GatewayControl() { }
        private double alarmThreshold = 10;
        private readonly IHttpClientFactory _clientFactory;
        
        public GatewayControl(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public static double calculateDistance(double x1, double y1, double x2, double y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        public GatewayModel findClosestGateway(MobileDevicesModel mobile)
        {
            double closest =9999.9;
            GatewayModel closestGateway = GatewayList[0];
            foreach (GatewayModel item in GatewayList)
            {
                if (!item.isActive)
                {
                    continue;
                }
                double value = calculateDistance(item.longitude, item.latitude, mobile.longitude, mobile.latitude);
                if (value < closest)
                {
                    closest = value;
                    closestGateway = item;
                }
            }
            return closestGateway;
        }
        public GatewayModel findClosestGateway(GatewayModel gateway)
        {
            double closest = 9999.9;
            GatewayModel closestGateway = GatewayList[0];
            foreach (GatewayModel item in GatewayList)
            {
                if (item.latitude == gateway.latitude && item.longitude == gateway.longitude)
                {
                    continue;
                }
                double value = calculateDistance(item.longitude, item.latitude, gateway.longitude, gateway.latitude);
                if (value < closest)
                {
                    closest = value;
                    closestGateway = item;
                }
            }
            return closestGateway;
        }
        public bool  findGateway(string gatewayId , out GatewayModel gateway)
        {
            if (GatewayDictionary.ContainsKey(gatewayId))
            {
                gateway = GatewayDictionary[gatewayId];
                return true;
            }
            gateway = new GatewayModel { gatewayId = "-1" };
            return false;
        }
        public void setClosestGatewayAlarm(GatewayModel  gateway)
        {
            foreach (GatewayModel item in GatewayList)
            {
                if (calculateDistance(gateway.latitude , item.latitude , gateway.longitude , item.longitude) <= alarmThreshold)
                {
                    item.isAlarm = true;
                    bool result;
                    do
                    {
                        result = SendToOtherGateway(gateway.gatewayUri, gateway.gatewayId, DateTime.Now);
                    } while (result == false);
                }
            }
        }
        public void setGatewayAlarm(string gateWayId, bool onFire)
        {
            throw new NotImplementedException();
        }
        public bool gatewayRegister(GatewayModel gatewayInfo)
        {
            if(GatewayDictionary.ContainsKey(gatewayInfo.gatewayId))
            {
                GatewayDictionary[gatewayInfo.gatewayId].UpdateTime = DateTime.Now;
                return GatewayDictionary[gatewayInfo.gatewayId].isAlarm;
            }
            else
            {
                GatewayDictionary[gatewayInfo.gatewayId] = gatewayInfo;
                return false;
            }
        }
        private bool SendToOtherGateway(string url, string gatewayId, DateTime time  )
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            GatewayMessageModel postData = new GatewayMessageModel() { gatewayId = gatewayId,  messageType = (int)messageCode.gateWayCode.fireAlarm , messageTime = time , content = $"附近的網關 { gatewayId } 號檢測到火災!" };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(url, contentPost).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GatewayMessageModel responseModel = JsonConvert.DeserializeObject<GatewayMessageModel>(cloudResponse);
            if (responseModel.content == "true")
            {
                return true;
            }
            return false;
        }

        public IReadOnlyList<GatewayModel> getGatewayList()
        {
            return GatewayList;
        }

        public void removeGateway(GatewayModel gateway)
        {
            GatewayDictionary.Remove(gateway.gatewayId);
        }
    }
}
