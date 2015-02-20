using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Caching;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Services
{
	public class MossServerInfoConfigurationElement : ServerInfoConfigureElement
	{
		/// <summary>
		/// Uri的地址字符串
		/// </summary>
		[ConfigurationProperty("baseUri")]
		private string BaseUriString
		{
			get
			{
				return (string)this["baseUri"];
			}
		}

		[ConfigurationProperty("documentLibraryName")]
		public string DocumentLibraryName
		{
			get
			{
				return (string)this["documentLibraryName"];
			}
		}

		/// <summary>
		/// 配置的Uri
		/// </summary>
		public Uri BaseUri
		{
			get
			{
				return UriContextCache.GetUri(this.BaseUriString);
			}
		}

		/// <summary>
		/// 全文检索的服务地址
		/// </summary>
		[ConfigurationProperty("searchServiceUrl")]
		public string MossSearchServiceUrl
		{
			get
			{
				return (string)this["searchServiceUrl"];
			}
		}

		[ConfigurationProperty("logListName")]
		public string LogListName
		{
			get
			{
				return (string)this["logListName"];
			}
		}
	}

	[ConfigurationCollection(typeof(MossServerInfoConfigurationElement),
		CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class MossServerInfoConfigrationElementCollection : NamedConfigurationElementCollection<MossServerInfoConfigurationElement>
	{
	}
}