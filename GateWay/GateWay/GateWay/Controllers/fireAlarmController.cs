using System;
using System.Net.Http;
using System.Text;
using GateWay.Models;
using GateWay.typeCode;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GateWay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class fireAlarmController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        public fireAlarmController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public ObjectResult getFireAlarm()//(fireAlarmModel fireAlarm)
        {
            //if(SendToCloud("azure/cloud/api", (int)messageCode.gateWayCode.fireAlarm, DateTime.Now))
            //{
                return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, content = "true" });
            //}
            //return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, content = "false" });
        }

        private bool SendToCloud(string url , int type , DateTime time)
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            gateWayMessageModel postData = new gateWayMessageModel() { gateWayId = typeCode.GateWay.gateWayId, messageType = type, messageTime = time };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(url, contentPost).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            cloudResponseModel responseModel = JsonConvert.DeserializeObject<cloudResponseModel>(cloudResponse);
            if (responseModel.content == "true")
            {
                return true;
            }
            return false;
        }
        private void SendToSensor()
        {

        }
    }
}
