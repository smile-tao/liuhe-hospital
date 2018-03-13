using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Founder.HisService
{
    public class LogMessage
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string businessType { get; set; }
        public string requestData { get; set; }
        public string responseData { get; set; }

        public override string ToString()
        {
            return  "userName"+ "[" + userName + "]" +
                    "password" + "[" + password + "]" +
                    "businessType" + "[" + businessType + "]" +
                    "requestData" + "[" + requestData + "]" +
                    "responseData" + "[" + responseData + "]";
        }
    }
}