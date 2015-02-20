using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 准备用户操作日志的事件定义
    /// </summary>
    /// <param name="dataContext"></param>
    /// <param name="logs"></param>
    public delegate void DesignerPrepareUserOperationLogEventHandler(WfDesignerExecutorDataContext dataContext, UserOperationLogCollection logs);

    /// <summary>
    /// Executor的通用事件定义
    /// </summary>
    /// <param name="dataContext"></param>
    public delegate void DesignerExecutorEventHandler(WfDesignerExecutorDataContext dataContext);

    /// <summary>
    /// 发生错误的事件定义
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="dataContext"></param>
    /// <param name="autoThrow"></param>
    public delegate void DesignerErrorEventHandler(System.Exception ex, WfDesignerExecutorDataContext dataContext, ref bool autoThrow);
}
