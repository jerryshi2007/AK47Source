using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// Html Header中定义的元素
	/// </summary>
	public abstract class HederElementsConfigurationElementBase : ConfigurationElement
	{
		[ConfigurationProperty("all", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public FilePathConfigElementCollection All
		{
			get
			{
				return (FilePathConfigElementCollection)this["all"];
			}
		}

		/// <summary>
		/// Css文件配置节集合
		/// </summary>
		[ConfigurationProperty("debug", IsRequired = false)]
		public FilePathConfigElementCollection Debug
		{
			get
			{
				return (FilePathConfigElementCollection)this["debug"];
			}
		}

		[ConfigurationProperty("release", IsRequired = false)]
		public FilePathConfigElementCollection Release
		{
			get
			{
				return (FilePathConfigElementCollection)this["release"];
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}
	}
}
