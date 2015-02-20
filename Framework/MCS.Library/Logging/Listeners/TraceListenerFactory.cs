#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	TraceListenerFactory.cs
// Remark	：	日志侦听器的工厂类
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// 1.1		    ccic\zhangtiejun	20080928		修改GetListeners方法
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    internal class TraceListenerFactory
    {
        private static ReaderWriterLock rwLock = new ReaderWriterLock();//modify by yuanyong 20070603
		private const int DefaultLockTimeout = 3000;//modify by yuanyong 20070603

        #region Get Listeners
        public static List<FormattedTraceListenerBase> GetListeners()
        {
            return new List<FormattedTraceListenerBase>();
        }

		public static List<FormattedTraceListenerBase> GetListeners(LoggerListenerConfigurationElementCollection listenerElements)
        {
			TraceListenerFactory.rwLock.AcquireWriterLock(TraceListenerFactory.DefaultLockTimeout);
            try
            {
                List<FormattedTraceListenerBase> listeners = new List<FormattedTraceListenerBase>();

                if (listenerElements != null)
                {
					foreach (LoggerListenerConfigurationElement listenerelement in listenerElements)
                    {
                        FormattedTraceListenerBase listener = GetListenerFromConfig(listenerelement);

                        if (listener != null)
                            listeners.Add(listener);
                    }
                }

                return listeners;
            }
            catch (Exception ex)
            {
                throw new LogException("创建Listeners时发生错误：" + ex.Message, ex);
            }
            finally
            {
				TraceListenerFactory.rwLock.ReleaseWriterLock();
            }
        }
        #endregion

		private static FormattedTraceListenerBase GetListenerFromConfig(LoggerListenerConfigurationElement element)
        {
            return (FormattedTraceListenerBase)TypeCreator.CreateInstance(element.Type, element);
        }
    }
}
