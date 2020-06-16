using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class GateWayModel
    {
        public string gateWayId { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string gateWayUri { get; set; }
        public bool isAlarm { get; set; }
    }
}
