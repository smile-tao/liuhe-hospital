using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Founder.HisService
{
    [ServiceContract]
    public interface IFounderService
    {
        [OperationContract]
        int FounderRequestData(string userName, string password, string businessType, string requestData, out string responseData);


    }
}
