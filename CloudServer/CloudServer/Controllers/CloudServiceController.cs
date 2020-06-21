using System;
using System.Net.Http;
using CloudServer.Code;
using CloudServer.Models;
using CloudServer.UtilComponent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CloudServiceController : ControllerBase
    {
        private readonly IGatewayControl _gatewayControl;

        public CloudServiceController(IGatewayControl gatewayControl , IHttpClientFactory clientFactory)
        {
            _gatewayControl = gatewayControl;
        }
        // gw 火災訊號
        [HttpPost]
        public IActionResult  getFireAlarmGateWay([FromBody]GatewayMessageModel gatewayMessage)
        {
            string gatewayId = gatewayMessage.gatewayId;
            GatewayModel alarmGateWay;
            _gatewayControl.findGateway(gatewayId, out alarmGateWay);
            if (gatewayMessage.messageType == 7)
            {
                alarmGateWay.isAlarm = false;
            }
            else
                alarmGateWay.isAlarm = true;
            return new OkObjectResult(new GatewayMessageModel { gatewayId = gatewayId, content = "true" });
        }
        // 手機找到最近gw
        [HttpPost]
        public IActionResult findGayWay([FromBody]MobileDevicesModel mobile)
        {
            GatewayModel cloesetGateway;
            cloesetGateway = _gatewayControl.findClosestGateway(mobile);
            return new OkObjectResult(new ReturnGatewayModel { gateWayId = cloesetGateway.gatewayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gatewayUri = cloesetGateway.gatewayUri , isAlarm = cloesetGateway.isAlarm });
        }
        // gw 註冊
        [HttpPost]
        public IActionResult gateWayRegister([FromBody]GatewayModel gatewayInfo)
        {
            gatewayInfo.UpdateTime = DateTime.Now;
            bool result =  _gatewayControl.gatewayRegister(gatewayInfo);
            return new OkObjectResult(new CloudResponseModel {messageType = (int)messageCode.gateWayCode.registerResponse, content = result.ToString() });
        }
        // 網頁控制點火
        [HttpPost]
        public IActionResult userControlFire(string onFireGatewayId)
        {
            GatewayModel alarmGateway;
            bool result;
            result = _gatewayControl.findGateway(onFireGatewayId, out alarmGateway);
            if (result)
            {
                alarmGateway.isAlarm = true;
            }
            return new OkObjectResult(new ReturnGatewayModel { gateWayId = onFireGatewayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gatewayUri = alarmGateway.gatewayUri, isAlarm = alarmGateway.isAlarm });
        }
        // 取得gw List
        [HttpPost]
        public IActionResult getGateWayList()
        {
            return new JsonResult(_gatewayControl.getGatewayList());
        }
    }
}
