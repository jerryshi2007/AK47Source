using System;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
	/// <summary>
	/// ��Udp Cache֪ͨ������ص�������Ϣ
	/// </summary>
	public sealed class UdpCacheClientSettings : ConfigurationSection
	{
		private UdpCacheClientSettings()
		{
		}

		/// <summary>
		/// ��ȡudpCacheNotifierSettings��������Ϣ
		/// </summary>
		/// <returns>UdpCacheNotifierSettingsʵ��</returns>
		public static UdpCacheClientSettings GetConfig()
		{
			UdpCacheClientSettings result = (UdpCacheClientSettings)ConfigurationBroker.GetSection("udpCacheClientSettings");

			if (result == null)
				result = new UdpCacheClientSettings();

			return result;
		}

		/// <summary>
		/// �õ��˿ںŵ�����
		/// </summary>
        /// <returns>�˿ںŵ�����</returns>
		public int[] GetPorts()
		{
			return UdpSettingsHelper.GetPorts(this.InnerPorts);
		}

		/// <summary>
		/// ip��ַ
		/// </summary>
		public IPAddress Address
		{
			get
			{
				return IPAddress.Parse(InnerIPAddress);
			}
		}

		/// <summary>
		/// ip��ַ
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
