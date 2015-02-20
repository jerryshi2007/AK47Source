using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
	public class CutomerServiceExecutiveSetting : ConfigurationSection
	{
		public static CutomerServiceExecutiveSetting GetConfig()
		{
			CutomerServiceExecutiveSetting settings = (CutomerServiceExecutiveSetting)ConfigurationBroker.GetSection("cutomerServiceExecutiveSetting");

			if (settings == null)
				settings = new CutomerServiceExecutiveSetting();

			return settings;
		}

		[ConfigurationProperty("impls")]
		private TypeConfigurationCollection Impls
		{
			get
			{
				return (TypeConfigurationCollection)this["impls"];
			}
		}

		[ConfigurationProperty("cutomerServiceExecutives")]
		public CutomerServiceExecutiveCollectio CutomerServiceExecutives
		{
			get
			{
				return (CutomerServiceExecutiveCollectio)this["cutomerServiceExecutives"];
			}
		}

		public ICutomerServiceExecutiveQuery CutomerServiceExecutiveQuery
		{
			get
			{
				ICutomerServiceExecutiveQuery result = null;

				if (Impls.ContainsKey("userOUControlQuery"))
					result = (ICutomerServiceExecutiveQuery)Impls["userOUControlQuery"].CreateInstance();
				else
					result = new DefaultCutomerServiceExecutiveQuery();

				return result;
			}
		}

		[ConfigurationProperty("title", IsRequired = false, DefaultValue = "流程客服")]
		public virtual string Title
		{
			get
			{
				return (string)this["title"];
			}
		}
	}

	public class CutomerServiceExecutiveElement : NamedConfigurationElement
	{
		[ConfigurationProperty("logOnName", IsRequired = true)]
		public virtual string LogOnName
		{
			get
			{
				return (string)this["logOnName"];
			}
		}

		[ConfigurationProperty("category", IsRequired = true)]
		public virtual string Category
		{
			get
			{
				return (string)this["category"];
			}
		}
	}

	public class CutomerServiceExecutiveCollectio : NamedConfigurationElementCollection<CutomerServiceExecutiveElement>
	{

	}
}
