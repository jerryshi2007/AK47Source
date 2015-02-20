#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	MetaConfigurationSourceInstanceSection.cs
// Remark	：	An SourceMappings configuration section
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.Diagnostics;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// SourceMappings配置节
    /// </summary>
    class MetaConfigurationSourceInstanceSection : ConfigurationSection 
    {
        /// <summary>
        /// Public const
        /// </summary>
        public const string Name = "sourceMappings";


        /// <summary>
        /// 所有实例的源映射元素集合
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceElementCollection.Name)]
        public MetaConfigurationSourceInstanceElementCollection Instances
        {
            get
            {
                return base[MetaConfigurationSourceInstanceElementCollection.Name] as
                    MetaConfigurationSourceInstanceElementCollection;
            }
        }

    }
}
