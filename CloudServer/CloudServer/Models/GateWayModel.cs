using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class GateWayModel
    {
        public string gateWayId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string gateWayUri { get; set; }
    }
}
