using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Founder.HisService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class FounderService : IFounderService
    {
        /// <summary>
        /// 服务入口
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="requestData">xml参数</param>
        /// <returns></returns>
        public int FounderRequestData(string userName, string password, string businessType, string requestData,
            out string responseData)
        {
            responseData = "";

            #region 定义日志消息体

            LogMessage logMessage = new LogMessage();
            logMessage.userName = userName;
            logMessage.password = password;
            logMessage.businessType = businessType;
            logMessage.requestData = requestData;

            #endregion

            if (businessType == "PACS_REQ") //获取申请单
            {
                return ECG.PACS_REQ(logMessage, out responseData);
            }
            else if (businessType == "PACS_CONFIRM") //申请单执行(确认)
            {
                return ECG.PACS_CONFIRM(logMessage, out responseData);
            }
            else if (businessType == "PACS_CANCEL") //申请单取消确认
            {
                return ECG.PACS_CANCEL(logMessage, out responseData);
            }
            else if (businessType == "Lis_Dictionary") //Lis字典同步
            {
                return ECG.Lis_Dictionary(logMessage, out responseData);
            }
            else if (businessType == "Lis_Patient") //Lis病人信息
            {
                return ECG.Lis_Patient(logMessage, out responseData);
            }
            else if (businessType == "Lis_Order") //Lis病区医嘱
            {
                return ECG.Lis_Order(logMessage, out responseData);
            }
            else if (businessType == "Lis_Mzcharge") //Lis门诊收费
            {
                return ECG.Lis_Mzcharge(logMessage, out responseData);
            }
            else if (businessType == "Lis_Confirm") //Lis确认记费
            {
                return ECG.Lis_Confirm(logMessage, out responseData);
            }
            else if (businessType == "GENERAL") //通用接口
            {
                return ECG.GENERAL(logMessage, out responseData);
            }
            else
            {
                responseData = string.Format("对于的业务代码[{0}]不存在", businessType);
                logMessage.responseData = responseData;
                Log4NetHelper.Info<string>(logMessage.ToString());
                return -1;
            }
        }
    }
}