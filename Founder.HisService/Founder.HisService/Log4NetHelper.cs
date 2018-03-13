using System;
using System.IO;
using System.Text;
using log4net;


namespace Founder.HisService
{

    public static class Log4NetHelper
    {
        static Log4NetHelper()
        {
            //初始化log4net配置     
     
            var config = ConfigContext.GetConfigContext().GetConfig("log4net");

            //重写log4net配置里的连接字符串
            config = config.Replace("{connectionString}", ConfigContext.GetConfigContext().config.DataBaseConfig);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(config));//这个地方的编译一定要和文件的编码一致
            log4net.Config.XmlConfigurator.Configure(ms);
        }

        public static void Info<T>(T message)
        {
       
            if (ConfigContext.GetConfigContext().config.TextLog)
            {
                log4net.ILog loggerLoginfo = log4net.LogManager.GetLogger("Loginfo");
                loggerLoginfo.Info(message);
     
          
            }
        
        }

            

    }

}
