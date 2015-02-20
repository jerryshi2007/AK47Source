#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	PriorityLogFilter.cs
// Remark	：	日志组件缺省实现的基于优先级的过滤器
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// 优先级过滤器
    /// </summary>
    /// <remarks>
    /// LogFilter的派生类，实现根据优先级过滤日志记录
    /// </remarks>
    public sealed class PriorityLogFilter : LogFilter
    {
        private LogPriority minPriority = LogPriority.Normal;

        private PriorityLogFilter()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public PriorityLogFilter(string name) : base(name)
        {
            
        }

        /// <summary>
        /// 重载的构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="minPriority">优先级阀值</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="增删LogFillter对象"></code>
        /// </remarks>
        public PriorityLogFilter(string name, LogPriority minPriority)
            : base(name)
        {
            this.minPriority = minPriority;
        }

        /// <summary>
        /// 重载的构造函数，从配置文件中读取、构造
        /// </summary>
        /// <param name="element">配置对象</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Filters\LogFilterFactory.cs" 
        /// lang="cs" region="Get FilterPipeline" title="获取Filter对象"></code>
        /// </remarks>
		public PriorityLogFilter(LoggerFilterConfigurationElement element)
            : base(element.Name)
        {
			this.minPriority = element.MinPriority;
        }

        /// <summary>
        /// 优先级阀值
        /// </summary>
        public LogPriority MinPriority
        {
            get
            {
                return this.minPriority;
            }
        }

        /// <summary>
        /// 覆写的方法，具体实现优先级过滤
        /// </summary>
        /// <param name="log">日志记录</param>
        /// <returns>布尔值，true：通过，false：不通过</returns>
        /// <remarks>
        /// 只有优先级大于等于minPriority的日志记录才能通过
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            return log.Priority >= this.minPriority;
        }
    }
}
