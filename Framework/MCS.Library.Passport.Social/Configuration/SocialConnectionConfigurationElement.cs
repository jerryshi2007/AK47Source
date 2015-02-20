using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Caching;
using MCS.Library.Configuration;

namespace MCS.Library.Passport.Social.Configuration
{
	/// <summary>
	/// 能够连接的社交网站的配置信息
	/// </summary>
	public class SocialConnectionConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// 图标的路径
		/// </summary>
		public Uri LogoPath
		{
			get
			{
				return UriContextCache.GetUri(this.LogoPathString);
			}
		}

		[ConfigurationProperty("logoPath", IsRequired = false)]
		private string LogoPathString
		{
			get
			{
				return (string)this["logoPath"];
			}
		}
	}

	/// <summary>
	/// 能够连接的社交网站的配置信息集合
	/// </summary>
	public class SocialConnectionConfigurationElementCollection : NamedConfigurationElementCollection<SocialConnectionConfigurationElement>
	{
	}
}
