using System;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 基于Udp的Cache通知器的配置信息
	/// </summary>
	public sealed class UdpCacheNotifierSettings : ConfigurationSection
	{
		private UdpCacheNotifierSettings()
		{
		}

		/// <summary>
		/// 获取udpCacheNotifierSettings的配置信息
		/// </summary>
		/// <returns>UdpCacheNotifierSettings实例</returns>
		public static UdpCacheNotifierSettings GetConfig()
		{
			UdpCacheNotifierSettings result = (UdpCacheNotifierSettings)ConfigurationBroker.GetSection("udpCacheNotifierSettings");

			if (result == null)
			{
				result = new UdpCacheNotifierSettings();
				result.EndPoints.InitDefaultData();
			}

			return result;
		}

		/// <summary>
		/// 数据包大小，缺省为1200个字节
		/// </summary>
		[ConfigurationProperty("packageSize", DefaultValue = 1200)]
		public int PackageSize
		{
			get
			{
				return (int)this["packageSize"];
			}
		}

		/// <summary>
		/// 发送的端点
		/// </summary>
		[ConfigurationProperty("endPoints")]
		public UdpCacheNotifierTargetCollection EndPoints
		{
			get
			{
				return (UdpCacheNotifierTargetCollection)this["endPoints"];
			}
		}
	}

	/// <summary>
	/// UdpCacheNotifierTarget集合
	/// </summary>
	public sealed class UdpCacheNotifierTargetCollection : ConfigurationElementCollection
	{
		internal UdpCacheNotifierTargetCollection()
		{
		}

		/// <summary>
		/// 得到元素的Key
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((UdpCacheNotifierTarget)element).Name;
		}

		/// <summary>
		/// 创建新元素
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new UdpCacheNotifierTarget();
		}

		internal void InitDefaultData()
		{
			UdpCacheNotifierTarget target = new UdpCacheNotifierTarget("broadcast");

			this.BaseAdd(target);
		}
	}

	/// <summary>
	/// 基于Udp的Cache通知器的发送目标
	/// </summary>
	public sealed class UdpCacheNotifierTarget : ConfigurationElement
	{
		internal UdpCacheNotifierTarget()
		{
		}

		internal UdpCacheNotifierTarget(string name)
		{
			this["name"] = name;
		}

		/// <summary>
		/// 名称
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey=true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
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
		/// 得到端口号的数组
		/// </summary>
		/// <returns></returns>
		public int[] GetPorts()
		{
			return UdpSettingsHelper.GetPorts(this.InnerPorts);
		}

		/// <summary>
		/// ip地址
		/// </summary>
		[ConfigurationProperty("address", DefaultValue = "255.255.255.255")]
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("Address: {0}", this.Address);

			int i = 0;
			foreach (int port in this.GetPorts())
			{
				if (i == 0)
					strB.Append(":");
				else
					strB.Append(",");

				strB.AppendFormat("{0}", port);
				i++;
			}

			return strB.ToString();
		}
	}
}
