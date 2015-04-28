#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	UriConfigurationElement.cs
// Remark	��	����Uri��������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
    /// ����Uri��������
    /// </summary>
    public class UriConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// Uri�ĵ�ַ�ַ���
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
        /// ���õ�Uri
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
    /// ����Uri���������
    /// </summary>
    public class UriConfigurationCollection : NamedConfigurationElementCollection<UriConfigurationElement>
    {
    }
}
