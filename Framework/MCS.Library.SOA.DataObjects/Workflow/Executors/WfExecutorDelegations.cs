using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 在Executor中，涉及到流转的事件定义
	/// </summary>
	/// <param name="dataContext"></param>
	/// <param name="eventArgs"></param>
	public delegate void MoveToEventHandler(WfExecutorDataContext dataContext, WfMoveToEventArgs eventArgs);

	/// <summary>
	/// 准备消息的事件定义
	/// </summary>
	/// <param name="dataContext"></param>
	/// <param name="tasks"></param>
	public delegate void PrepareTasksEventHandler(WfExecutorDataContext dataContext, UserTaskCollection tasks);

	/// <summary>
	/// Executor的通用事件定义
	/// </summary>
	/// <param name="dataContext"></param>
	public delegate void ExecutorEventHandler(WfExecutorDataContext dataContext);

	/// <summary>
	/// 准备用户操作日志的事件定义
	/// </summary>
	/// <param name="dataContext"></param>
	/// <param name="logs"></param>
	public delegate void PrepareUserOperationLogEventHandler(WfExecutorDataContext dataContext, UserOperationLogCollection logCollection);

	/// <summary>
	/// 发生错误的事件定义
	/// </summary>
	/// <param name="ex"></param>
	/// <param name="dataContext"></param>
	/// <param name="autoThrow"></param>
	public delegate void ErrorEventHandler(System.Exception ex, WfExecutorDataContext dataContext, ref bool autoThrow);
}
