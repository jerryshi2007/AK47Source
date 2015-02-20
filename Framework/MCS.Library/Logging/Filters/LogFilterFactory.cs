#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	LogFilterFactory.cs
// Remark	：	日志过滤器器的工厂类
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// 1.1		    ccic\zhangtiejun	20080928		修改GetFilterPipeLine方法
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;

using MCS.Library.Core;

namespace MCS.Library.Logging
{
    internal class LogFilterFactory
    {
        //private static ReaderWriterLock rwlock = new ReaderWriterLock();
		//private const int _defaultLockTimeout = 3000;

        #region Get FilterPipeline
        public static LogFilterPipeline GetFilterPipeLine()
        {
            return new LogFilterPipeline();
        }

		public static LogFilterPipeline GetFilterPipeLine(LoggerFilterConfigurationElementCollection filterElements)
        {
            //rwlock.AcquireReaderLock(defaultLockTimeout);
            try
            {
                LogFilterPipeline pipeline = new LogFilterPipeline();
                
                if (filterElements != null)
                {
					foreach (LoggerFilterConfigurationElement ele in filterElements)
                    {
                        ILogFilter filter = GetFilterFromConfig(ele);

                        if (filter != null)
                            pipeline.Add(filter);
                    }
                }

                return pipeline;
            }
            catch (Exception ex)
            {
                throw new LogException("创建FilterPipeline时发生错误：" + ex.Message, ex);
            }
            //finally
            //{
            //    rwlock.ReleaseReaderLock(); 
            //}
        }
        #endregion

		private static ILogFilter GetFilterFromConfig(LoggerFilterConfigurationElement element)
        {
            return (ILogFilter)TypeCreator.CreateInstance(element.Type, element);
        }
    }
}
