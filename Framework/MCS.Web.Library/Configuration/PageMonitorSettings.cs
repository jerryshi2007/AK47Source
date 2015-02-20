using System;
using System.Web;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class PageMonitorSettings : DeluxeConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static PageMonitorSettings GetConfig()
		{
			PageMonitorSettings config = (PageMonitorSettings)ConfigurationBroker.GetSection("pageMonitorSettings");

			if (config == null)
				config = new PageMonitorSettings();

			return config;
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("pages")]
		public PageMonitorElementCollection Pages
		{
			get
			{
				return (PageMonitorElementCollection)this["pages"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PageMonitorElementCollection : NamedConfigurationElementCollection<PageMonitorElement>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public PageMonitorElement GetMatchedElement()
		{
			PageMonitorElement result = null;

			HttpRequest request = HttpContext.Current.Request;

			foreach (PageMonitorElement pme in this)
			{
				if (pme.IsUrlMatched() && pme.IsVerbMatched())
				{
					result = pme;
					break;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PageMonitorElement : NamedConfigurationElement
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsUrlMatched()
		{
			return HttpContext.Current.Request.Url.PathAndQuery.IndexOf(UrlIncluding, 0, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsVerbMatched()
		{
			string verbFormatted = Verb.Trim();

			bool result = false;

			if (verbFormatted == "*")
				result = true;
			else
			{
				string[] verbs = verbFormatted.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

				result = Array.Exists(verbs, delegate(string v) 
							{ return string.Compare(v, HttpContext.Current.Request.HttpMethod, true) == 0; });
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("urlIncluding", IsRequired = true)]
		public string UrlIncluding
		{
			get
			{
				return (string)this["urlIncluding"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("enableLogging", IsRequired = false, DefaultValue = "true")]
		public bool EnableLogging
		{
			get
			{
				return (bool)this["enableLogging"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("enablePFCounter", IsRequired = false, DefaultValue = "true")]
		public bool EnablePFCounter
		{
			get
			{
				return (bool)this["enablePFCounter"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("counternInstance", IsRequired = false, DefaultValue = "")]
		public string CounterInstanceName
		{
			get
			{
				return (string)this["counternInstance"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("verb", IsRequired = false, DefaultValue = "*")]
		public string Verb
		{
			get
			{
				return (string)this["verb"];
			}
		}
	}
}
