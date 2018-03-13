using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Founder.HisService
{
    /// <summary>
    /// 获取XML值
    /// </summary>
    public class XmlGetValueHelper
    {
        /// <summary>
        /// 获取XML节点值
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static string GetValue(XmlNode xmlNode)
        {

            if (xmlNode == null)
            {
                return "";
            }
            else
            {
                return xmlNode.InnerText;
            
            }
        }
    }
}