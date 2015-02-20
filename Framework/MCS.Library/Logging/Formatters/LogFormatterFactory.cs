#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogFormatterFactory.cs
// Remark	��	��־��ʽ�����Ĺ����࣬�������ô�����ʽ����
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// 1.1		    ccic\zhangtiejun	20080928		�޸�GetFormatter����
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
                    throw new LogException("����Formatter����ʱ����" + ex.Message, ex);
                }
            }
            else
                return null;
        }
    }
}
