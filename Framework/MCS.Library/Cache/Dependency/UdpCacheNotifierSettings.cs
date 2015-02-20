using System;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
	/// <summary>
	/// ����Udp��Cache֪ͨ����������Ϣ
	/// </summary>
	public sealed class UdpCacheNotifierSettings : ConfigurationSection
	{
		private UdpCacheNotifierSettings()
		{
		}

		/// <summary>
		/// ��ȡudpCacheNotifierSettings��������Ϣ
		/// </summary>
		/// <returns>UdpCacheNotifierSettingsʵ��</returns>
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
		/// ���ݰ���С��ȱʡΪ1200���ֽ�
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
		/// ���͵Ķ˵�
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
	/// UdpCacheNotifierTarget����
	/// </summary>
	public sealed class UdpCacheNotifierTargetCollection : ConfigurationElementCollection
	{
		internal UdpCacheNotifierTargetCollection()
		{
		}

		/// <summary>
		/// �õ�Ԫ�ص�Key
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((UdpCacheNotifierTarget)element).Name;
		}

		/// <summary>
		/// ������Ԫ��
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
	/// ����Udp��Cache֪ͨ���ķ���Ŀ��
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
		/// ����
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
		/// �õ��˿ںŵ�����
		/// </summary>
		/// <returns></returns>
		public int[] GetPorts()
		{
			return UdpSettingsHelper.GetPorts(this.InnerPorts);
		}

		/// <summary>
		/// ip��ַ
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
