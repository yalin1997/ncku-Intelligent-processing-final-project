using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class ReturnGatewayModel
    {
        public string gateWayId { get; set; }
        public int messageType { get; set; }
        public string gatewayUri { get; set; }
        public bool isAlarm { get; set; }
    }
}
