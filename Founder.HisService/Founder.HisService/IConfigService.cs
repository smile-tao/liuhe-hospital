using System;
using System.Collections.Generic;

namespace Founder.HisService
{

    public interface IConfigService
    {
        /// <summary>
        /// 获取xml内容
        /// </summary>
        /// <param name="name">xml文件名称</param>
        /// <returns></returns>
        string GetConfig(string name);
        /// <summary>
        /// 保存xml内容
        /// </summary>
        /// <param name="name">xml文件名称</param>
        /// <param name="content">xml内容</param>
        void SaveConfig(string name, string content);
        /// <summary>
        /// 获取xml文件路径
        /// </summary>
        /// <param name="name">xml文件名称</param>
        /// <returns></returns>
        string GetFilePath(string name);
    }
}
