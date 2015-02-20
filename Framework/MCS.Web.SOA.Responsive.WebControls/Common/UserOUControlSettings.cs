using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.Responsive.WebControls
{
	public sealed class UserOUControlSettings : ConfigurationSection
	{
		public static UserOUControlSettings GetConfig()
		{
			UserOUControlSettings settings = (UserOUControlSettings)ConfigurationBroker.GetSection("userOUControlSettings");

			if (settings == null)
				settings = new UserOUControlSettings();

			return settings;
		}

		private UserOUControlSettings()
		{
		}

		[ConfigurationProperty("impls")]
		private TypeConfigurationCollection Impls
		{
			get
			{
				return (TypeConfigurationCollection)this["impls"];
			}
		}

		public IUserOUControlQuery UserOUControlQuery
		{
			get
			{
				IUserOUControlQuery result = null;

				if (Impls.ContainsKey("userOUControlQuery"))
					result = (IUserOUControlQuery)Impls["userOUControlQuery"].CreateInstance();
				else
					result = OriginaUserOUControlQueryImpl.Instance;

				return result;
			}
		}
	}
}
