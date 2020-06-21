using GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    public interface IAuthenticateControl
    {
        public bool authDeviceInfo(AccountPasswordModel deviceInfo);
    }
}
