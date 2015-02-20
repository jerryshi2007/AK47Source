using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Cache通知的附加设置
    /// </summary>
    public sealed class CacheNotifierSettings : ConfigurationSection
    {
        private CacheNotifierSettings()
        {
        }

        /// <summary>
        /// 获取cacheNotifierSettings的配置信息
        /// </summary>
        /// <returns>UdpCacheNotifierSettings实例</returns>
        public static CacheNotifierSettings GetConfig()
        {
            CacheNotifierSettings result = (CacheNotifierSettings)ConfigurationBroker.GetSection("cacheNotifierSettings");

            if (result == null)
                result = new CacheNotifierSettings();

            return result;
        }

        /// <summary>
        /// 将接收到的Udp通知转发到内存映射文件的通知项中
        /// </summary>
        [ConfigurationProperty("forwardUdpToMmf", DefaultValue = false)]
        public bool ForwardUdpToMmf
        {
            get
            {
                return (bool)this["forwardUdpToMmf"];
            }
        }

		/// <summary>
		/// 是否启用内存映射文件的通知
		/// </summary>
		[ConfigurationProperty("enableMmfNotifier", DefaultValue = false)]
		public bool EnableMmfNotifier
		{
			get
			{
				return (bool)this["enableMmfNotifier"];
			}
		}

        /// <summary>
        /// 返回true。主要是当配置文件改了之后为了保持兼容性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }

        /// <summary>
        /// 返回true。主要是当配置文件改了之后为了保持兼容性
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            return true;
        }
    }
}
