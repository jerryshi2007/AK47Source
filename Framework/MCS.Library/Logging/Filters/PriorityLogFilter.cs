#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	PriorityLogFilter.cs
// Remark	��	��־���ȱʡʵ�ֵĻ������ȼ��Ĺ�����
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ���ȼ�������
    /// </summary>
    /// <remarks>
    /// LogFilter�������࣬ʵ�ָ������ȼ�������־��¼
    /// </remarks>
    public sealed class PriorityLogFilter : LogFilter
    {
        private LogPriority minPriority = LogPriority.Normal;

        private PriorityLogFilter()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public PriorityLogFilter(string name) : base(name)
        {
            
        }

        /// <summary>
        /// ���صĹ��캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="minPriority">���ȼ���ֵ</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public PriorityLogFilter(string name, LogPriority minPriority)
            : base(name)
        {
            this.minPriority = minPriority;
        }

        /// <summary>
        /// ���صĹ��캯�����������ļ��ж�ȡ������
        /// </summary>
        /// <param name="element">���ö���</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Filters\LogFilterFactory.cs" 
        /// lang="cs" region="Get FilterPipeline" title="��ȡFilter����"></code>
        /// </remarks>
		public PriorityLogFilter(LoggerFilterConfigurationElement element)
            : base(element.Name)
        {
			this.minPriority = element.MinPriority;
        }

        /// <summary>
        /// ���ȼ���ֵ
        /// </summary>
        public LogPriority MinPriority
        {
            get
            {
                return this.minPriority;
            }
        }

        /// <summary>
        /// ��д�ķ���������ʵ�����ȼ�����
        /// </summary>
        /// <param name="log">��־��¼</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        /// <remarks>
        /// ֻ�����ȼ����ڵ���minPriority����־��¼����ͨ��
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            return log.Priority >= this.minPriority;
        }
    }
}
