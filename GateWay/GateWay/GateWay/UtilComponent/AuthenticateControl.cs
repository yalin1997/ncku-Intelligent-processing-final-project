using GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateWay.UtilComponent
{
    public class AuthenticateControl : IAuthenticateControl
    {
        private List<accountPasswordModel> memberList = new List<accountPasswordModel>();
        public AuthenticateControl()
        {
            
        }
        public bool authDeviceInfo(accountPasswordModel deviceInfo)
        {
            foreach(accountPasswordModel item in memberList)
            {
                if (deviceInfo.Auth(item))
                {
                    return true;
                }
                continue;
            }
            return false;
        }
    }
}
