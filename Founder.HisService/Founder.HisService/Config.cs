using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Founder.HisService
{
    /// <summary>
    /// 程序读取配置类,如果加入别的配置只要在这个地方加入一个属性就可以了
    /// </summary>
    [XmlRoot("config")]
    public class Config
    {
        /// <summary>
        /// 程序数据库
        /// </summary>
        [XmlElement("DataBaseConfig")]
        public string DataBaseConfig { get; set; }
        [XmlElement("TextLog")]
        public Boolean TextLog { get; set; }
        [XmlElement("DbLog")]
        public Boolean DbLog { get; set; }

        [XmlElement("DetailHours")]
        public int DetailHours { get; set; }

    }
}
