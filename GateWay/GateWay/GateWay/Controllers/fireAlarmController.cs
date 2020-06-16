using System;
using System.Net.Http;
using System.Text;
using CloudServer.Models;
using GateWay.Models;
using GateWay.typeCode;
using GateWay.UtilComponent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration Configuration;
        public fireAlarmController(IHttpClientFactory clientFactory , IAlarmControl fireAlarmControl, IAuthenticateControl authControl , IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _alarmControl = fireAlarmControl;
            _authControl = authControl;
            Configuration = configuration;
        }
        [HttpPost]
        public IActionResult getFireAlarm([FromBody]accountPasswordModel deviceMessage)//(fireAlarmModel fireAlarm)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                if (SendAlarmToCloud((int)messageCode.gateWayCode.fireAlarm, DateTime.Now))
                {
                    return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.alarmResponse , content = "true" });
                }
            }
            return new ObjectResult("auth error");
        }
        [HttpPost]
        public IActionResult getFireAlarmFromCloud([FromBody] gateWayMessageModel cloudMessage)//(fireAlarmModel fireAlarm)
        {
            _alarmControl.setAlarm();
             return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.alarmResponse, content = "true" });
        }
        [HttpPost]
        public IActionResult cloudFireAlarm([FromBody] accountPasswordModel cloudMessage)
        {
            if (_authControl.authDeviceInfo(cloudMessage))
            {
                _alarmControl.setAlarm();
                return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.alarmResponse, content = "true" });
            }
            return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.alarmResponse, content = "false" });
        }
        [HttpPost]
        public IActionResult SensorAlarm([FromBody] accountPasswordModel deviceMessage)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                if (_alarmControl.isAlarm())
                {
                    return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.sensorAlarm, content = "true" });
                }
                return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.sensorAlarm, content = "false" });
            }
            return new ObjectResult("auth error");
        }
        public IActionResult sendAlive()
        {
            return new OkObjectResult(new gateWayMessageModel { gateWayId = Configuration["GateWayId"], messageType = (int)messageCode.gateWayCode.alive, content = "true" });
        }

        private bool SendAlarmToCloud( int type , DateTime time)
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            gateWayMessageModel postData = new gateWayMessageModel() { gateWayId = Configuration["GateWayId"], messageType = type, messageTime = time };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(Configuration["CloudUri"] + "/api/fireAlarm/gateWayRegister", contentPost).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            cloudResponseModel responseModel = JsonConvert.DeserializeObject<cloudResponseModel>(cloudResponse);
            if (responseModel.content == "true")
            {
                return true;
            }
            return false;
        }
        private bool SendRegisterToCloud()
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            GateWayModel postData = new GateWayModel() { gateWayId = Configuration["GateWayId"], longitude = Convert.ToDouble(Configuration["longitude"]), latitude = Convert.ToDouble(Configuration["latitude"]) };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(Configuration["CloudUri"]+ "/api/fireAlarm/gateWayRegister", contentPost).GetAwaiter().GetResult();
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
