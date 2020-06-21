using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class GatewayModel
    {
        public string gatewayId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string gatewayUri { get; set; }
        public bool isAlarm { get; set; }
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public bool isActive { get => (DateTime.Now - UpdateTime).TotalMinutes < 3; }
    }
}
