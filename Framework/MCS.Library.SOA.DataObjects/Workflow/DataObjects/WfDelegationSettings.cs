using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 委托待办信息的设置
	/// </summary>
	public sealed class WfDelegationSettings : ConfigurationSection
	{
		private WfDelegationSettings()
		{
		}

		public static WfDelegationSettings GetConfig()
		{
			WfDelegationSettings result =
				(WfDelegationSettings)ConfigurationBroker.GetSection("wfDelegationSettings");

			if (result == null)
				result = new WfDelegationSettings();

			return result;
		}

		public IWfDelegationReader Reader
		{
			get
			{
				IWfDelegationReader result = null;

				if (OperationElements.ContainsKey("delegationReader"))
					result = (IWfDelegationReader)OperationElements["delegationReader"].CreateInstance();
				else
					result = WfDelegationAdapter.Instance;

				return result;
			}
		}

		[ConfigurationProperty("operations")]
		private TypeConfigurationCollection OperationElements
		{
			get
			{
				return (TypeConfigurationCollection)this["operations"];
			}
		}
	}
}
