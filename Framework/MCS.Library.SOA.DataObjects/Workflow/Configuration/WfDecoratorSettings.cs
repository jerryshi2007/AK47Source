using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程修饰器的配置信息
	/// </summary>
	public sealed class WfDecoratorSettings : ConfigurationSection
	{
		private WfProcessDecoratorCollection _Decorators = null;
		private object _SyncObject = new object();

		public static WfDecoratorSettings GetConfig()
		{
			WfDecoratorSettings settings = (WfDecoratorSettings)ConfigurationBroker.GetSection("wfDecoratorSettings");

			if (settings == null)
				settings = new WfDecoratorSettings();

			return settings;
		}

		private WfDecoratorSettings()
		{
		}

		public WfProcessDecoratorCollection GetDecorators()
		{
			if (this._Decorators == null)
			{
				lock (this._SyncObject)
				{
					if (this._Decorators == null)
						this._Decorators = GetConfigedDecorators();
				}
			}

			return this._Decorators;
		}

		[ConfigurationProperty("decorators")]
		private TypeConfigurationCollection Decorators
		{
			get
			{
				return (TypeConfigurationCollection)this["decorators"];
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
		{
			return true;
		}

		private WfProcessDecoratorCollection GetConfigedDecorators()
		{
			WfProcessDecoratorCollection result = new WfProcessDecoratorCollection();

			foreach (TypeConfigurationElement typeElement in this.Decorators)
				result.Add(typeElement.CreateInstance<IWfProcessDecorator>());

			return result;
		}
	}
}
