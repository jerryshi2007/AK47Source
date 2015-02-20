#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogFilterPipeline.cs
// Remark	��	��־�������ܵ���Pipeline�����Ƕ����־����������ϣ�ֻ��ÿ����������ͨ���˲���ͨ���ܵ�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;
using MCS.Library.Accessories;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ʵ��ILogFilter�Ĺܵ���Pipeline��
    /// </summary>
    /// <remarks>
    /// ����LogFilter���϶���
    /// </remarks>
#if DELUXEWORKSTEST
    public class LogFilterPipeline : FilterPipelineBase<ILogFilter, LogEntity>
#else
    internal class LogFilterPipeline : FilterPipelineBase<ILogFilter, LogEntity>
#endif
    {
        private static readonly object _syncObject = new object();

        internal LogFilterPipeline(List<ILogFilter> filters)
        {
            this.pipeline = filters;
        }

        internal LogFilterPipeline()
        {
            this.pipeline = new List<ILogFilter>();
        }

        /// <summary>
        /// ��Pipeline�����ILogFilter����
        /// </summary>
        /// <param name="filter">ILogFilter����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public override void Add(ILogFilter filter)
        {
            lock (_syncObject)
            {
                pipeline.Add(filter);
            }
        }

        /// <summary>
        /// ��Pipeline���Ƴ�ILogFilterʵ��
        /// </summary>
        /// <param name="filter">ILogFilterʵ��</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public override void Remove(ILogFilter filter)
        {
            lock (_syncObject)
            {
                pipeline.Remove(filter);
            }
        }

        #region �����������
        /// <summary>
        /// �����������ص�����ILogFilterʵ��
        /// </summary>
        /// <param name="index">����</param>
        /// <returns>������ILogFilterʵ��</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public ILogFilter this[int index]
        {
            get
            {
                return pipeline[index];
            }
        }

        /// <summary>
        /// Pipeline��ILogFilterʵ���ĸ���
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LogFilterPipelineTest.cs"
        /// lang="cs" region="ILogFilter AddRemove Test" tittle="��ɾLogFillter����"></code>
        /// </remarks>
        public int Length
        {
            get
            {
                return pipeline.Count;
            }
        }
        #endregion

        /// <summary>
        /// ʵ����־���ˣ��ж��Ƿ�ͨ��LogFilterPipeline
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        /// <remarks>
        /// Pipeline��Filter֮���ǡ��롱�Ĺ�ϵ��ֻ�����е�Filter��ͨ��������ͨ��
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" tittle="д��־��¼"></code>
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            bool passFilters = true;

            if (this.pipeline != null)
            {
                foreach (ILogFilter filter in pipeline)
                {
                    try
                    {
                        bool passed = filter.IsMatch(log);
                        passFilters &= passed;

                        if (false == passFilters)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new LogException(string.Format("LogFilter:{0}������־ʱʧ�ܣ����������û�ʵ���Ƿ���ȷ", filter.Name), ex);
                    }
                }
            }
            return passFilters;
        }
    }
}
