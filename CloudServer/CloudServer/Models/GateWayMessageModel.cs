using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServer.Models
{
    public class GateWayMessageModel
    {
        public string gateWayId { get; set; }
        public int messageType { get; set; }
        public DateTime messageTime { get; set; }
        public string content { get; set; }
    }
}
