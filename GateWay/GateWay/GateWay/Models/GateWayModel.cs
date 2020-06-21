using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class GatewayModel
    {
        public string gatewayId { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string gatewayUri { get; set; }
        public bool isAlarm { get; set; }
    }
}
