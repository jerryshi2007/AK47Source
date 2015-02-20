using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    /// <summary>
    /// 日志提供程序配置
    /// </summary>
    public sealed class LoggerConfigurationElement : CustomTypeConfigurationElementBase
    {
    }

    /// <summary>
    /// 日志提供程序配置的集合
    /// </summary>
    public class LoggerConfigurationElementCollection : CustomTypeConfigurationElementCollection<LoggerConfigurationElement>
    {

    }
}
