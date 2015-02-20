using System;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 与Udp Cache通知机制相关的配置信息
	/// </summary>
	public sealed class UdpCacheClientSettings : ConfigurationSection
	{
		private UdpCacheClientSettings()
		{
		}

		/// <summary>
		/// 获取udpCacheNotifierSettings的配置信息
		/// </summary>
		/// <returns>UdpCacheNotifierSettings实例</returns>
		public static UdpCacheClientSettings GetConfig()
		{
			UdpCacheClientSettings result = (UdpCacheClientSettings)ConfigurationBroker.GetSection("udpCacheClientSettings");

			if (result == null)
				result = new UdpCacheClientSettings();

			return result;
		}

		/// <summary>
		/// 得到端口号的数组
		/// </summary>
        /// <returns>端口号的数组</returns>
		public int[] GetPorts()
		{
			return UdpSettingsHelper.GetPorts(this.InnerPorts);
		}

		/// <summary>
		/// ip地址
		/// </summary>
		public IPAddress Address
		{
			get
			{
				return IPAddress.Parse(InnerIPAddress);
			}
		}

		/// <summary>
		/// ip地址
		/// </summary>
		[ConfigurationProperty("address", DefaultValue="0.0.0.0")]
		private string InnerIPAddress
		{
			get
			{
				return (string)this["address"];
			}
		}

		[ConfigurationProperty("ports", DefaultValue = "8080-8099")]
		private string InnerPorts
		{
			get
			{
				return (string)this["ports"];
			}
		}
	}
}
