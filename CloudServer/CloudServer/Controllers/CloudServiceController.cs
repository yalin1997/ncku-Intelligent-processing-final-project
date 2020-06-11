using System;
using System.Collections.Generic;
using System.Linq;
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
        public CloudServiceController(IGateWayControl gatewayControl)
        {
            _gatewayControl = gatewayControl;
        }
        [HttpPost]
        public void getFireAlarmGateWay([FromBody]GateWayMessageModel gatewayMessage)
        {
            
        }
        [HttpPost]
        public ObjectResult findGayWay(MobileDevicesModel mobile)
        {
            GateWayModel cloesetGateWay;
            cloesetGateWay = _gatewayControl.findClosestGateWay(mobile);
            return new OkObjectResult(new ReturnGateWayModel { gateWayId = cloesetGateWay.gateWayId, messageType = (int)messageCode.gateWayCode.gateWayReponse, gateWayUri = cloesetGateWay.gateWayUri });
        }
        [HttpPost]
        public ObjectResult gateWayRsgister(GateWayModel gatewayInfo)
        {
           bool result =  _gatewayControl.gateWayRegister(gatewayInfo);
            return new OkObjectResult(new CloudResponseModel {messageType = (int)messageCode.gateWayCode.registerResponse, content = result.ToString() });
        }
    }
}
