using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;
using System.Globalization;
using MCS.Library.Caching;

namespace MCS.Library.Globalization
{
	/// <summary>
	/// 翻译器的配置节
	/// </summary>
	public sealed class TranslatorConfigSettings : DeluxeConfigurationSection
	{
		/// <summary>
		/// 得到配置节
		/// </summary>
		/// <returns></returns>
		public static TranslatorConfigSettings GetConfig()
		{
			TranslatorConfigSettings result = (TranslatorConfigSettings)ConfigurationBroker.GetSection("translatorConfigSettings");

			if (result == null)
				result = new TranslatorConfigSettings();

			return result;
		}

		/// <summary>
		/// 缺省的文种
		/// </summary>
		public CultureInfo DefaultCulture
		{
			get
			{
				CultureInfo result;

				if (CultureInfoContextCache.Instance.TryGetValue(DefaultCultureString, out result) == false)
				{
					result = new CultureInfo(DefaultCultureString);

					CultureInfoContextCache.Instance.Add(DefaultCultureString, result);
				}

				return result;
			}
		}

		/// <summary>
		/// 默认的Culture
		/// </summary>
		[ConfigurationProperty("defaultCulture", IsRequired = false, DefaultValue = "zh-CN")]
		private string DefaultCultureString
		{
			get
			{
				return (string)this["defaultCulture"];
			}
		}

		/// <summary>
		/// 翻译器
		/// </summary>
		public ITranslator Translator
		{
			get
			{
				ITranslator translator = GetTranslator();

				if (translator == null)
					translator = new DefaultTranslator();

				return translator;
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

		private ITranslator GetTranslator()
		{
			ITranslator result = null;

			foreach (TypeConfigurationElement element in this.TypeFactories)
			{
				try
				{
					result = (ITranslator)element.CreateInstance();
					break;
				}
				catch (TypeLoadException)
				{
				}
			}

			return result;
		}
	}

	internal sealed class CultureInfoContextCache : ContextCacheQueueBase<string, CultureInfo>
	{
		public static CultureInfoContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<CultureInfoContextCache>();
			}
		}

		private CultureInfoContextCache()
		{
		}
	}
}
