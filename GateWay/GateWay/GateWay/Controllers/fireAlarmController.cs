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
    public class FireAlarmController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAlarmControl _alarmControl;
        private readonly IAuthenticateControl _authControl;
        private readonly IConfiguration Configuration;
        public FireAlarmController(IHttpClientFactory clientFactory , IAlarmControl fireAlarmControl, IAuthenticateControl authControl , IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _alarmControl = fireAlarmControl;
            _authControl = authControl;
            Configuration = configuration;
        }
        // sensor to gw
        [HttpPost]
        public IActionResult getFireAlarm([FromBody]AccountPasswordModel deviceMessage)//(fireAlarmModel fireAlarm)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                _alarmControl.setAlarm();
                bool result;
                do
                {
                    result = SendAlarmToCloud((int)MessageCode.gatewayCode.fireAlarm, DateTime.Now);
                } while (!result);
                return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.alarmResponse , content = "true" });

            }
            return new ObjectResult("auth error");
        }
        //停止火災警報
        [HttpPost]
        public IActionResult fireAlarmStop([FromBody] AccountPasswordModel deviceMessage)//(fireAlarmModel fireAlarm)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                _alarmControl.setSafe();
                bool result;
                do
                {
                    result = SendAlarmToCloud((int)MessageCode.gatewayCode.stopAlarm, DateTime.Now);
                } while (!result);
                return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.alarmResponse, content = "true" });
            }
            return new ObjectResult("auth error");
        }
        // 收不到
        [HttpPost]
        public IActionResult getFireAlarmFromCloud([FromBody] GatewayMessageModel cloudMessage)//(fireAlarmModel fireAlarm)
        {
            _alarmControl.setAlarm();
             return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.alarmResponse, content = "true" });
        }
        [HttpPost]
        // 收不到
        public IActionResult cloudFireAlarm([FromBody] AccountPasswordModel cloudMessage)
        {
            if (_authControl.authDeviceInfo(cloudMessage))
            {
                _alarmControl.setAlarm();
                return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.alarmResponse, content = "true" });
            }
            return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.alarmResponse, content = "false" });
        }
        // 給sensor問
        [HttpPost]
        public IActionResult SensorAlarm([FromBody] AccountPasswordModel deviceMessage)
        {
            if (_authControl.authDeviceInfo(deviceMessage))
            {
                if (_alarmControl.isAlarm())
                {
                    return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.sensorAlarm, content = "true" });
                }
                return new OkObjectResult(new GatewayMessageModel { gatewayId = _alarmControl.getId(), messageType = (int)MessageCode.gatewayCode.sensorAlarm, content = "false" });
            }
            return new ObjectResult("auth error");
        }

        private bool SendAlarmToCloud( int type , DateTime time)
        {
            var cloudHttpSender = _clientFactory.CreateClient();
            GatewayMessageModel postData = new GatewayMessageModel() { gatewayId = _alarmControl.getId(), messageType = type, messageTime = time };
            // 將 data 轉為 json
            string json = JsonConvert.SerializeObject(postData);
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = cloudHttpSender.PostAsync(Configuration["CloudUri"] + "/api/CloudService/getFireAlarmGateWay", contentPost).GetAwaiter().GetResult();
            string cloudResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            CloudResponseModel responseModel = JsonConvert.DeserializeObject<CloudResponseModel>(cloudResponse);
            if (responseModel.content == "true")
            {
                return true;
            }
            return false;
        }

    }
}
