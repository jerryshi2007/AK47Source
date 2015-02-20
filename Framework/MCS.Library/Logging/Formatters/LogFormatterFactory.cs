#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	LogFormatterFactory.cs
// Remark	：	日志格式化器的工厂类，根据配置创建格式化器
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// 1.1		    ccic\zhangtiejun	20080928		修改GetFormatter方法
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    internal class LogFormatterFactory
    {
		public static ILogFormatter GetFormatter(LoggerFormatterConfigurationElement formatterElement)
        {
            if (formatterElement != null)
            {
                try
                {
                    return (ILogFormatter)TypeCreator.CreateInstance(formatterElement.Type, formatterElement);
                }
                catch (Exception ex)
                {
                    throw new LogException("创建Formatter对象时出错：" + ex.Message, ex);
                }
            }
            else
                return null;
        }
    }
}
