using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 页面扩展信息配置节
	/// </summary>
	public class PageContentSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// 样式的文件集合
		/// </summary>
		[ConfigurationProperty("cssClasses", IsRequired = false)]
		public CssClassesConfigurationElement CssClasses
		{
			get
			{
				return (CssClassesConfigurationElement)this["cssClasses"];
			}
		}

		[ConfigurationProperty("scripts", IsRequired = false)]
		public ScriptsConfigurationElement Scripts
		{
			get
			{
				return (ScriptsConfigurationElement)this["scripts"];
			}
		}

		/// <summary>
		/// 不自动加载的页面配置节集合
		/// </summary>
		[ConfigurationProperty("notAutoLoadPages", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public NotAutoLoadPageConfigElementCollection NotAutoLoadPages
		{
			get
			{
				return (NotAutoLoadPageConfigElementCollection)this["notAutoLoadPages"];
			}
		}

		/// <summary>
		/// 自动加密内容的控件ID
		/// </summary>
		[ConfigurationProperty("autoEncryptControlIDs", DefaultValue = "")]
		public string AutoEncryptControlIDs
		{
			get
			{
				return (string)this["autoEncryptControlIDs"];
			}
		}

		/// <summary>
		/// 是否自动为每个页面加载扩展
		/// </summary>
		[ConfigurationProperty("autoLoad", DefaultValue = false)]
		public bool AutoLoad
		{
			get
			{
				return (bool)this["autoLoad"];
			}
		}
	}

	/// <summary>
	/// 不自动加载的页面配置节集合
	/// </summary>
	public class NotAutoLoadPageConfigElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new NotAutoLoadPageConfigElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NotAutoLoadPageConfigElement)element).Path;
		}
	}

	/// <summary>
	/// 不自动加载的页面配置节
	/// </summary>
	public class NotAutoLoadPageConfigElement : ConfigurationElement
	{
		/// <summary>
		/// page路径
		/// </summary>
		[ConfigurationProperty("path", DefaultValue = "")]
		public string Path
		{
			get
			{
				return (string)this["path"];
			}
		}
	}
}
