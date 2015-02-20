using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 工作流Action配置
	/// </summary>
	public sealed class WfActionSettings : ConfigurationSection
	{
		public static WfActionSettings GetConfig()
		{
			WfActionSettings settings = (WfActionSettings)ConfigurationBroker.GetSection("wfActionSettings");

			if (settings == null)
				settings = new WfActionSettings();

			return settings;
		}

		private WfActionSettings()
		{
		}

		public IWfAction GetAction(string actionName)
		{
			TypeConfigurationElement actionConfig = Actions[actionName];

			(actionConfig != null).FalseThrow<SettingsPropertyNotFoundException>("不能在actionSettings中找到名称为{0}的Action", actionName);

			return actionConfig.CreateInstance() as IWfAction;
		}

		[ConfigurationProperty("actions")]
		private TypeConfigurationCollection Actions
		{
			get
			{
				return (TypeConfigurationCollection)this["actions"];
			}
		}

		public IEnumerable<IWfCalculateUserFunction> Functions
		{
			get
			{
				IEnumerable<IWfCalculateUserFunction> result = (IEnumerable<IWfCalculateUserFunction>)ObjectContextCache.Instance.GetOrAddNewValue(
					"WfActionSettings.Functions",
					(cache, key) =>
					{
						List<IWfCalculateUserFunction> funcInstances = new List<IWfCalculateUserFunction>();

						foreach (TypeConfigurationElement elem in FunctionElements)
							funcInstances.Add((IWfCalculateUserFunction)elem.CreateInstance());

						cache.Add(key, funcInstances);

						return funcInstances;
					});

				return result;
			}
		}

		/// <summary>
		/// 系统定制的功能函数，用于条件计算等自定义函数的实现
		/// </summary>
		[ConfigurationProperty("functions")]
		private TypeConfigurationCollection FunctionElements
		{
			get
			{
				return (TypeConfigurationCollection)this["functions"];
			}
		}
	}
}
