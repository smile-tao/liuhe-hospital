using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Founder.HisService
{
    /// <summary>
    /// 使用单例模式保存配置文件,只要一次加载就可以了
    /// </summary>
    public class ConfigContext
    {
        private readonly string configFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

        public Config config = null;
        private static ConfigContext configContext = new ConfigContext();//单例

        private IConfigService ConfigService = null;

        private ConfigContext()
        {
            ConfigService = new FileConfigService();
            //读取配置文件,转换成Model
            config = XmlHelper.XmlDeserialize<Config>
                (ConfigService.GetConfig("ConfigContext"), Encoding.UTF8);         
        }
        public static ConfigContext GetConfigContext() {

            return configContext;
        }

        public string GetConfig(string name)
        {
            return ConfigService.GetConfig(name);
            
        }
        public string GetFilePath(string fileName)
        {
            var configPath = string.Format(@"{0}\{1}.xml", configFolder, fileName);
            return configPath;
        }
    }

}
