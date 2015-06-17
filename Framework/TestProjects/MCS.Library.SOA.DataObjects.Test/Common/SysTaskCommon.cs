using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Data;
using System.Diagnostics;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
    /// 与任务相关辅助方法(已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test)
	/// </summary>
	public static class SysTaskCommon
	{
		public static int ExecuteAllTasks()
		{
			int count = 0;
			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				SysTaskCollection tasks = null;
				do
				{
					tasks = SysTaskAdapter.Instance.FetchNotRuningSysTasks(-1, (task) =>
					{
						ExecuteAndAssertTask(task);
					});

					count += tasks.Count;
				}
				while (tasks.Count > 0);
			}
			finally
			{
				sw.Stop();
				Console.WriteLine("总共执行了{0}个任务，耗时{1:#,##0}毫秒", count, sw.ElapsedMilliseconds);
			}

			return count;
		}

		/// <summary>
		/// 执行并且验证Task的返回结果
		/// </summary>
		/// <param name="func"></param>
		public static void ExecuteAndAssertTask(SysTask task)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				SysTask taskLoaded = SysTaskAdapter.Instance.Load(task.TaskID);

				ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(taskLoaded.TaskType);

				Console.WriteLine("执行任务：ID={0}, Name={1}, Type={2}",
					taskLoaded.TaskID, taskLoaded.TaskTitle, taskLoaded.TaskType);

				executor.Execute(taskLoaded);

				SysAccomplishedTask accomplishedTask = SysAccomplishedTaskAdapter.Instance.Load(taskLoaded.TaskID);

				Assert.IsNotNull(accomplishedTask);

				if (accomplishedTask.StatusText.IsNotEmpty())
					Console.WriteLine(accomplishedTask.StatusText);

				Assert.AreEqual(SysTaskStatus.Completed, accomplishedTask.Status);
			}
			finally
			{
				sw.Stop();
				Console.WriteLine("执行任务：ID={0}经过的时间为{1:#,##0毫秒}", task.TaskID, sw.ElapsedMilliseconds);
				Console.WriteLine();
			}
		}

		public static void ExecuteTask(SysTask task)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				SysTask taskLoaded = SysTaskAdapter.Instance.Load(task.TaskID);

				ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(taskLoaded.TaskType);

				Console.WriteLine("执行任务：ID={0}, Name={1}, Type={2}",
					taskLoaded.TaskID, taskLoaded.TaskTitle, taskLoaded.TaskType);

				executor.Execute(taskLoaded);

				SysAccomplishedTask accomplishedTask = SysAccomplishedTaskAdapter.Instance.Load(taskLoaded.TaskID);

				Assert.IsNotNull(accomplishedTask);

				if (accomplishedTask.StatusText.IsNotEmpty())
					Console.WriteLine(accomplishedTask.StatusText);
			}
			finally
			{
				sw.Stop();
				Console.WriteLine("执行任务：ID={0}经过的时间为{1:#,##0毫秒}", task.TaskID, sw.ElapsedMilliseconds);
				Console.WriteLine();
			}
		}
	}
}
