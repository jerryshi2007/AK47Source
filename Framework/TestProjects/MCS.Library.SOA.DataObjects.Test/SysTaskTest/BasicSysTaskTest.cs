using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DO = MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.SysTaskTest
{
    /// <summary>
    /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
    /// </summary>
	[TestClass]
	public class BasicSysTaskTest
	{
		[TestMethod]
		[TestCategory("SysTask")]
		[Description("SysTask的基本读写库操作")]
		public void CreateSimpleSysTask()
		{
			DO.SysTask task = new DO.SysTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "新任务",
			};

			DO.SysTaskAdapter.Instance.Update(task);

			DO.SysTask taskLoaded = DO.SysTaskAdapter.Instance.Load(task.TaskID);

			Assert.IsNotNull(taskLoaded);
			Assert.AreEqual(task.TaskID, taskLoaded.TaskID);
			Assert.AreEqual(task.TaskTitle, taskLoaded.TaskTitle);
		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("将未运行或运行中的SysTask移到SYS_ACCOMPLISHED_TASK表中")]
		public void SetSysTaskToCompleted()
		{
			SetSysTaskToCompletedSub(new DO.SysTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "新任务2",
				Status = SysTaskStatus.NotRunning
			}, SysTaskStatus.Aborted, "Test Aborted");
		}

		private static void SetSysTaskToCompletedSub(DO.SysTask task1, SysTaskStatus status, string statusText)
		{
			DO.SysTaskAdapter.Instance.Update(task1);

			SysTaskAdapter.Instance.MoveToCompletedSysTask(task1.TaskID, status, statusText);

			DO.SysTask task11 = SysTaskAdapter.Instance.Load(task1.TaskID);

			Assert.IsNull(task11);

			var task12 = SysAccomplishedTaskAdapter.Instance.Load(task1.TaskID);

			Assert.IsNotNull(task12);

			Assert.AreEqual(status, task12.Status);
		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("读取未运行的任务")]
		public void FetchNotRunningTasks()
		{
			SysTaskAdapter.Instance.ClearAll();

			for (int i = 0; i < 8; i++)
			{
				var task = new DO.SysTask()
				{
					TaskID = UuidHelper.NewUuidString(),
					TaskTitle = "新任务"
				};

				SysTaskAdapter.Instance.Update(task);
			}

			SysTaskAdapter.Instance.FetchNotRuningSysTasks(2, m =>
			{
				SysTaskAdapter.Instance.MoveToCompletedSysTask(m.TaskID, SysTaskStatus.Aborted, "Unit Test Error");
			});

			var result = SysTaskAdapter.Instance.FetchNotRuningSysTasks(8, null);

			Assert.AreEqual(6, result.Count);
		}
	}
}
