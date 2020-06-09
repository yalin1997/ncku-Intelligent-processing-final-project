using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GateWay.Models
{
    public class accountPasswordModel
    {
        public string account { get; set; }
        public string password { get; set;}
        public bool Auth(accountPasswordModel obj)
        {
            if (this.account.Equals(obj.account) && this.password.Equals(obj.password))
            {
                return true;
            }
            return false;
        }
    }
}
