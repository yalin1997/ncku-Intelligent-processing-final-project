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
        public IActionResult  getFireAlarmGateWay([FromBody]GateWayMessageModel gatewayMessage)
        {
            string gateWayId = gatewayMessage.gateWayId;
            GateWayModel alarmGateWay;
            _gatewayControl.findGateWay(gateWayId, out alarmGateWay);
            if (gatewayMessage.messageType == 7)
            {
                alarmGateWay.isAlarm = false;
            }
            else
                alarmGateWay.isAlarm = true;
            return new OkObjectResult(new GateWayMessageModel { gateWayId = gateWayId, content = "true" });
        }
        // 手機找到最近gw
        [HttpPost]
        public IActionResult findGayWay([FromBody]MobileDevicesModel mobile)
        {
            GateWayModel cloesetGateWay;
            cloesetGateWay = _gatewayControl.findClosestGateWay(mobile);
            return new OkObjectResult(new ReturnGateWayModel { gateWayId = cloesetGateWay.gateWayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gateWayUri = cloesetGateWay.gateWayUri , isAlarm = cloesetGateWay.isAlarm });
        }
        // gw 註冊
        [HttpPost]
        public IActionResult gateWayRegister([FromBody]GateWayModel gatewayInfo)
        {
            gatewayInfo.UpdateTime = DateTime.Now;
            bool result =  _gatewayControl.gateWayRegister(gatewayInfo);
            return new OkObjectResult(new CloudResponseModel {messageType = (int)messageCode.gateWayCode.registerResponse, content = result.ToString() });
        }
        // 網頁控制點火
        [HttpPost]
        public IActionResult userControlFire(string onFireGateWayId)
        {
            GateWayModel alarmGateWay;
            bool result;
            result = _gatewayControl.findGateWay(onFireGateWayId, out alarmGateWay);
            if (result)
            {
                alarmGateWay.isAlarm = true;
            }
            return new OkObjectResult(new ReturnGateWayModel { gateWayId = onFireGateWayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gateWayUri = alarmGateWay.gateWayUri, isAlarm = alarmGateWay.isAlarm });
        }
        // 取得gw List
        [HttpPost]
        public IActionResult getGateWayList()
        {
            return new JsonResult(_gatewayControl.getGateWayList());
        }
    }
}
