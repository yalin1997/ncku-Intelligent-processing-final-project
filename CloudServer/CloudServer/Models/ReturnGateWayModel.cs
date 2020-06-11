using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class ReturnGateWayModel
    {
        public string gateWayId { get; set; }
        public int messageType { get; set; }
        public string gateWayUri { get; set; }
    }
}
