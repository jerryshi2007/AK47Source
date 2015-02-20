using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Library
{
	public sealed class ViewStatePersistSettings : ConfigurationSection
	{
		public static ViewStatePersistSettings GetConfig()
		{
			ViewStatePersistSettings settings = (ViewStatePersistSettings)ConfigurationBroker.GetSection("viewStatePersistSettings");

			if (settings == null)
				settings = new ViewStatePersistSettings();

			return settings;
		}

		private ViewStatePersistSettings()
		{
		}

		/// <summary>
		/// 是否使用特定的Persister
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = "false", IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 使用特定的Persister的阈值，大于此阈值才使用，阈值的单位是KB
		/// </summary>
		[ConfigurationProperty("threshold", DefaultValue = "10240", IsRequired = false)]
		public int Threshold
		{
			get
			{
				return (int)this["threshold"];
			}
		}

		/// <summary>
		/// 页面ViewState的持久化器
		/// </summary>
		public PageStatePersister Persister
		{
			get
			{
				PageStatePersister result = null;

				if (TypeFactories.Count > 0)
					result = (PageStatePersister)TypeFactories[0].CreateInstance((Page)HttpContext.Current.CurrentHandler);
				else
					result = new SqlPageStatePersister((Page)HttpContext.Current.CurrentHandler);

				return result;
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}
	}
}
