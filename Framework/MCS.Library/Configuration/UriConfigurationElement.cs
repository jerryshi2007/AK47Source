#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	UriConfigurationElement.cs
// Remark	：	关于Uri的配置项
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// 关于Uri的配置项
    /// </summary>
    public class UriConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// Uri的地址字符串
        /// </summary>
        [ConfigurationProperty("uri")]
        protected string UriString
        {
            get
            {
                return (string)this["uri"];
            }
        }

        /// <summary>
        /// 配置的Uri
        /// </summary>
        public Uri Uri
        {
            get
            {
                return UriContextCache.GetUri(this.UriString);
            }
        }
    }

    /// <summary>
    /// 关于Uri的配置项集合
    /// </summary>
    public class UriConfigurationCollection : NamedConfigurationElementCollection<UriConfigurationElement>
    {
    }
}
