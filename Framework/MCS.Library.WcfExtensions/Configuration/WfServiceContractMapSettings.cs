using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.WcfExtensions.Configuration
{
	public class WfServiceContractMapSettings : ConfigurationSection
	{
		private WfServiceContractMapElementCollection _Maps;
		private object _SyncRoot = new object();

		public static WfServiceContractMapSettings GetConfig()
		{
			WfServiceContractMapSettings result = (WfServiceContractMapSettings)ConfigurationBroker.GetSection("wfServiceContractMapSettings");
			if (result == null)
			{
				result = new WfServiceContractMapSettings();
			}

			return result;
		}

		[ConfigurationProperty("maps")]
		public WfServiceContractMapElementCollection Maps
		{
			get
			{
				lock (this._SyncRoot)
				{
					if (this._Maps == null)
					{
						this._Maps = (WfServiceContractMapElementCollection)this["maps"];
						if (this._Maps == null)
							this._Maps = new WfServiceContractMapElementCollection();
					}

					return this._Maps;
				}
			}
		}
	}

	public class WfServiceContractMapElementCollection : NamedConfigurationElementCollection<WfServiceContractMapElement>
	{

	}

	public enum WfServiceBindingMode
	{
		WfRawContentWebHttpBinding = 0,
		BasicHttpBinding = 1,
		WSHttpBinding = 2
	}

	public class WfServiceContractMapElement : NamedConfigurationElement
	{
		[ConfigurationProperty("serviceName", IsKey = true, IsRequired = true)]
		public override string Name
		{
			get
			{
				return (string)this["serviceName"];
			}
		}

		[ConfigurationProperty("contractName", IsRequired = true)]
		public string ContractName
		{
			get
			{
				return (string)this["contractName"];
			}
		}

		/// <summary>
		/// 服务是否返回调试信息
		/// </summary>
		[ConfigurationProperty("debug", IsRequired = false, DefaultValue = false)]
		public bool Debug
		{
			get
			{
				return (bool)this["debug"];
			}
		}

		/// <summary>
		/// 服务是否支持Asp.net Ajax调用，如果为true则包含一个/atlas的服务地址
		/// </summary>
		[ConfigurationProperty("atlasEnabled", IsRequired = false, DefaultValue = false)]
		public bool AtlasEnabled
		{
			get
			{
				return (bool)this["atlasEnabled"];
			}
		}

		/// <summary>
		/// 如果没有配置Endpoint模式，自动初始化的Endpoint的模式
		/// </summary>
		[ConfigurationProperty("bindingMode", IsRequired = false, DefaultValue = WfServiceBindingMode.WfRawContentWebHttpBinding)]
		public WfServiceBindingMode BindingMode
		{
			get
			{
				return (WfServiceBindingMode)this["bindingMode"];
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}
	}
}
