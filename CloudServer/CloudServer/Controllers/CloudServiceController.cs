using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly IGateWayControl _gatewayControl;

        public CloudServiceController(IGateWayControl gatewayControl , IHttpClientFactory clientFactory)
        {
            _gatewayControl = gatewayControl;
        }
        [HttpPost]
        public OkObjectResult getFireAlarmGateWay([FromBody]GateWayMessageModel gatewayMessage)
        {
            string gateWayId = gatewayMessage.gateWayId;
            GateWayModel alarmGateWay;
            _gatewayControl.findGateWay(gateWayId, out alarmGateWay);
            return new OkObjectResult(new GateWayMessageModel { gateWayId = gateWayId, content = "true" });
        }
        [HttpPost]
        public ObjectResult findGayWay([FromBody]MobileDevicesModel mobile)
        {
            GateWayModel cloesetGateWay;
            cloesetGateWay = _gatewayControl.findClosestGateWay(mobile);
            return new OkObjectResult(new ReturnGateWayModel { gateWayId = cloesetGateWay.gateWayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gateWayUri = cloesetGateWay.gateWayUri , isAlarm = cloesetGateWay.isAlarm });
        }
        [HttpPost]
        public ObjectResult gateWayRsgister([FromBody]GateWayModel gatewayInfo)
        {
           bool result =  _gatewayControl.gateWayRegister(gatewayInfo);
            return new OkObjectResult(new CloudResponseModel {messageType = (int)messageCode.gateWayCode.registerResponse, content = result.ToString() });
        }

        
    }
}
