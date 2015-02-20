using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Web.Library.MVC
{
	public sealed class OpenFormSettings : ConfigurationSection
	{
		private OpenFormSettings()
		{
		}

		public static OpenFormSettings GetConfig()
		{
			OpenFormSettings result =
				(OpenFormSettings)ConfigurationBroker.GetSection("openFormSettings");

			if (result == null)
				result = new OpenFormSettings();

			return result;
		}

		/// <summary>
		/// 消息操作类
		/// </summary>
		public IEnumerable<IUserProcessAclChecker> AclCheckers
		{
			get
			{
				if (AclCheckerElements.Count > 0)
				{
					foreach (TypeConfigurationElement elem in AclCheckerElements)
					{
						IUserProcessAclChecker checker = null;

						try
						{
							checker = (IUserProcessAclChecker)elem.CreateInstance();
						}
						catch (TypeLoadException)
						{
						}

						if (checker != null)
							yield return checker;
					}
				}
				else
				{
					yield return DefaultAdminUserProcessAclChecker.Instance;
					yield return DefaultQueryUserProcessAclChecker.Instance;
					yield return DefaultUserProcessAclChecker.Instance;
				}
			}
		}

		[ConfigurationProperty("aclCheckers")]
		private TypeConfigurationCollection AclCheckerElements
		{
			get
			{
				return (TypeConfigurationCollection)this["aclCheckers"];
			}
		}
	}
}
