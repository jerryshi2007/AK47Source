using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 同步执行的系统任务的基类
	/// </summary>
	public abstract class SyncSysTaskExecutorBase : ISysTaskExecutor
	{
		#region ISysTaskExecutor Members
		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="task"></param>
		public void BeforeExecute(SysTask task)
		{
			OnBeforeExecute(task);
		}

		/// <summary>
		/// 同步执行作业
		/// </summary>
		/// <param name="task"></param>
		/// <returns>已经完成的任务</returns>
		public SysAccomplishedTask Execute(SysTask task)
		{
			SysAccomplishedTask result = null;

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				BeforeExecute(task);
				OnExecute(task);
				result = OnAfterExecute(task);
			}
			catch (System.Exception ex)
			{
				result = OnError(task, ex);
			}
			finally
			{
				sw.Stop();

				Debug.WriteLine("执行任务，Duration={0:#,##0}ms, ID={1}, Name={2}, Type={3}",
					sw.ElapsedMilliseconds, task.TaskID, task.TaskTitle, task.TaskType);
			}

			return result;
		}
		#endregion
		/// <summary>
		/// 执行之前，置执行状态
		/// </summary>
		/// <param name="task"></param>
		protected virtual void OnBeforeExecute(SysTask task)
		{
			// 修改Task的状态为Running
			if (task.StartTime == DateTime.MinValue)
				task.StartTime = DateTime.Now;

			SysTaskAdapter.Instance.UpdateStatus(task.TaskID, SysTaskStatus.Running);
		}

		/// <summary>
		/// 执行具体的内容
		/// </summary>
		/// <param name="task"></param>
		protected abstract void OnExecute(SysTask task);

		/// <summary>
		/// 执行之后，置执行状态，并且移动到已完成中
		/// </summary>
		/// <param name="task"></param>
		/// <returns>返回的已完成任务</returns>
		protected virtual SysAccomplishedTask OnAfterExecute(SysTask task)
		{
			//Move,修改Task的状态为Completed
			return SysTaskAdapter.Instance.MoveToCompletedSysTask(task, SysTaskStatus.Completed, string.Empty);
		}

		/// <summary>
		/// 出现异常，置执行状态，并且移动到已完成中
		/// </summary>
		/// <param name="task"></param>
		/// <param name="ex"></param>
		/// <returns>返回的已完成任务</returns>
		protected virtual SysAccomplishedTask OnError(SysTask task, System.Exception ex)
		{
			//Move,修改Task的状态为Aborted
			return SysTaskAdapter.Instance.MoveToCompletedSysTask(task, SysTaskStatus.Aborted, ex.GetRealException().ToString());
		}
	}
}
