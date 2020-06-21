﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWay.Models
{
    public class GatewayMessageModel
    {
        public string gatewayId { get; set; }
        public int messageType { get; set; }
        public DateTime messageTime { get; set; }
        public string content { get; set; }

    }
}
