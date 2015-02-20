using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class UserTaskOperationSettings : ConfigurationSection
	{
		private UserTaskOperationSettings()
		{ }

		public static UserTaskOperationSettings GetConfig()
		{
			UserTaskOperationSettings result =
				(UserTaskOperationSettings)ConfigurationBroker.GetSection("soaUserTaskOperationSettings");

			if (result == null)
				result = new UserTaskOperationSettings();

			return result;
		}

		/// <summary>
		/// 消息操作类
		/// </summary>
		public IEnumerable<IUserTaskOperation> Operations
		{
			get
			{
				if (OperationElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in OperationElements)
						yield return (IUserTaskOperation)elem.CreateInstance();
				}
				else
					yield return new DefaultUserTaskOperationImpl();
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
