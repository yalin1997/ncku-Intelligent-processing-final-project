using System;
using System.Net.Http;
using System.Text;
using GateWay.Models;
using GateWay.typeCode;
using GateWay.UtilComponent;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GateWay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class fireAlarmController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAlarmControl _alarmControl;
        private readonly IAuthenticateControl _authControl;
        public fireAlarmController(IHttpClientFactory clientFactory , IAlarmControl fireAlarmControl, IAuthenticateControl authControl)
        {
            _clientFactory = clientFactory;
            _alarmControl = fireAlarmControl;
            _authControl = authControl;
        }
        [HttpPost]
        public ObjectResult getFireAlarm([FromBody]accountPasswordModel deviceMessage)//(fireAlarmModel fireAlarm)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                if (SendAlarmToCloud("azure/cloud/api", (int)messageCode.gateWayCode.fireAlarm, DateTime.Now))
                {
                    return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.alarmResponse , content = "true" });
                }
            }
            return new ObjectResult("auth error");
        }
        [HttpPost]
        public ObjectResult cloudFireAlarm([FromBody] accountPasswordModel cloudMessage)
        {
            if (_authControl.authDeviceInfo(cloudMessage))
            {
                _alarmControl.setAlarm();
                return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.alarmResponse, content = "true" });
            }
            return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.alarmResponse, content = "false" });
        }
        [HttpPost]
        public ObjectResult SensorAlarm([FromBody] accountPasswordModel deviceMessage)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                if (_alarmControl.isAlarm())
                {
                    return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.sensorAlarm, content = "true" });
                }
                return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.sensorAlarm, content = "false" });
            }
            return new ObjectResult("auth error");
        }
        public ObjectResult sendAlive()
        {
            return new OkObjectResult(new gateWayMessageModel { gateWayId = typeCode.GateWay.gateWayId, messageType = (int)messageCode.gateWayCode.alive, content = "true" });
        }

        private bool SendAlarmToCloud(string url , int type , DateTime time)
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

    }
}
