using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DO = MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.SysTaskTest
{
	[TestClass]
	public class JobRelatedTaskTest
	{
		[TestMethod]
		[TestCategory("SysTask")]
		[Description("测试配置信息")]
		public void SysTaskSettingsTest()
		{
			Assert.IsNotNull(SysTaskSettings.GetSettings().GetExecutor("InvokeService"));
			Assert.IsNotNull(SysTaskSettings.GetSettings().GetExecutor("StartWorkflow"));
		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("测试调用服务的任务")]
		public void InvokeServiceTaskTest()
		{
			//准备Job信息
			//调用SysTaskSettings.GetSettings().GetExecutor("InvokeService")执行服务
			//检验状态

			InvokeWebServiceJob job = new InvokeWebServiceJob()
			{
				JobID = UuidHelper.NewUuidString(),
				JobType = JobType.InvokeService,
				Category = "测试权限中心服务",
				Description = "仅用于测试",
				Enabled = true,
				Name = "任务和作业的单元测试",
				SvcOperationDefs = new Workflow.WfServiceOperationDefinitionCollection()
			};

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			parameters.Add(new WfServiceOperationParameter() { Name = "callerID", Type = WfSvcOperationParameterType.RuntimeParameter, Value = "callerID" });

			job.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(new Workflow.WfServiceAddressDefinition(Workflow.WfServiceRequestMethod.Post, null,
				"http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx"), "GetVersion", parameters, string.Empty));
			InvokeWebServiceJobAdapter.Instance.Update(job);

			DO.SysTask task = new DO.SysTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "测试任务",
				ResourceID = job.JobID
			};

			SysTaskAdapter.Instance.Update(task);

			ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor("InvokeService");

			executor.Execute(task);

			var task2 = SysTaskAdapter.Instance.Load(task.TaskID);

			Assert.IsNull(task2);
		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("测试调用大量服务的任务")]
		public void InvokeHugeServiceTasksTest()
		{
			SysTaskAdapter.Instance.ClearAll();
			SysAccomplishedTaskAdapter.Instance.ClearAll();

			InvokeWebServiceJob job = new InvokeWebServiceJob()
			{
				JobID = UuidHelper.NewUuidString(),
				JobType = JobType.InvokeService,
				Category = "测试权限中心服务",
				Description = "仅用于测试",
				Enabled = true,
				Name = "任务和作业的单元测试",
				SvcOperationDefs = new Workflow.WfServiceOperationDefinitionCollection()
			};

			job.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(new Workflow.WfServiceAddressDefinition(Workflow.WfServiceRequestMethod.Post, null, "http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx"), "GetVersion", null, string.Empty));
			InvokeWebServiceJobAdapter.Instance.Update(job);

			for (int i = 0; i < 400; i++)
			{
				DO.SysTask task = new DO.SysTask()
				{
					TaskID = UuidHelper.NewUuidString(),
					TaskTitle = "测试任务" + i,
					ResourceID = job.JobID,
					TaskType = "InvokeService"
				};

				SysTaskAdapter.Instance.Update(task);
			}

			int count = 0;
			var tasks = SysTaskAdapter.Instance.FetchNotRuningSysTasks(800, m =>
			{
				SysTaskAdapter.Instance.UpdateStatus(m.TaskID, SysTaskStatus.Running);

			});
			Thread thread = Thread.CurrentThread;

			Assert.AreEqual(400, tasks.Count);

			Debug.WriteLine(DateTime.Now + "开始执行");
			Stopwatch watch = new Stopwatch();
			watch.Start();

			var result = System.Threading.Tasks.Parallel.ForEach<DO.SysTask>(tasks, t =>
			{
				if (t.TaskType == "InvokeService")
				{
					Debug.WriteLine(DateTime.Now + "当前执行：" + System.Threading.Interlocked.Increment(ref count) + "  主线程状态" + thread.ThreadState + "当前线程ID：" + Thread.CurrentThread.ManagedThreadId);
					ISysTaskExecutor exec = SysTaskSettings.GetSettings().GetExecutor("InvokeService");
					exec.Execute(t);
				}
			});

			watch.Stop();

			Debug.WriteLine(string.Format("已完成所有任务: {0} 耗时 {1}ms", result.IsCompleted, watch.ElapsedMilliseconds));
			Debug.WriteLine(DateTime.Now + "主线程执行结束");



		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("测试调用大量服务的任务")]
		public void InvokeHugeServiceTasksTest2()
		{
			SysTaskAdapter.Instance.ClearAll();
			SysAccomplishedTaskAdapter.Instance.ClearAll();

			InvokeWebServiceJob job = new InvokeWebServiceJob()
			{
				JobID = UuidHelper.NewUuidString(),
				JobType = JobType.InvokeService,
				Category = "测试权限中心服务",
				Description = "仅用于测试",
				Enabled = true,
				Name = "任务和作业的单元测试",
				SvcOperationDefs = new Workflow.WfServiceOperationDefinitionCollection()
			};

			job.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(new Workflow.WfServiceAddressDefinition(Workflow.WfServiceRequestMethod.Post, null, "http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx"), "GetVersion", null, string.Empty));
			InvokeWebServiceJobAdapter.Instance.Update(job);

			for (int i = 0; i < 1500; i++)
			{
				DO.SysTask task = new DO.SysTask()
				{
					TaskID = UuidHelper.NewUuidString(),
					TaskTitle = "测试任务" + i,
					ResourceID = job.JobID,
					TaskType = "InvokeService"
				};

				SysTaskAdapter.Instance.Update(task);
			}

			int count = 0;

			var tasks = SysTaskAdapter.Instance.FetchNotRuningSysTasks(-1, m =>
			{
				SysTaskAdapter.Instance.UpdateStatus(m.TaskID, SysTaskStatus.Running);
			});

			Debug.WriteLine(DateTime.Now + "开始执行");
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (AutoResetEvent eventAuto = new AutoResetEvent(false))
			{
				foreach (var task in tasks)
				{
					ThreadPool.QueueUserWorkItem(item =>
					{
						DO.SysTask curTask = (DO.SysTask)item;
						if (curTask.TaskType == "InvokeService")
						{
							try
							{
								ISysTaskExecutor exec = SysTaskSettings.GetSettings().GetExecutor("InvokeService");
								exec.Execute(curTask);
							}
							catch (Exception exx)
							{
								Debug.WriteLine("出现了意外的错误" + exx.ToString());
							}
							finally
							{
								int newCount = System.Threading.Interlocked.Increment(ref count);
								Debug.WriteLine(DateTime.Now + "当前执行：" + newCount + "当前线程ID：" + Thread.CurrentThread.ManagedThreadId);
								eventAuto.Set();
							}
						}
					}, task);
				}

				int workerThreads, completionPortThreads;
				ThreadPool.GetAvailableThreads(out  workerThreads, out completionPortThreads);

				Debug.WriteLine("线程池可用线程数" + workerThreads);

				while (count != tasks.Count)
				{
					eventAuto.WaitOne();
				}
			}

			watch.Stop();

			Debug.WriteLine(string.Format("耗时 {0}ms", watch.ElapsedMilliseconds));
			Debug.WriteLine(DateTime.Now + "主线程执行结束");
		}

		/*
		[TestMethod]
		[TestCategory("SysTask")]
		[Description("线程池测试")]
		public void MaxPoolTest()
		{
			int count = 0;
			Stopwatch watch = new Stopwatch();
			watch.Start();

			using (AutoResetEvent eventOne = new AutoResetEvent(false))
			{
				for (int i = 0; i < 65536; i++)
				{
					bool enp = ThreadPool.QueueUserWorkItem(o =>
					{
						Thread.Sleep(100);
						Interlocked.Increment(ref count);
						eventOne.Set();
					}, i.ToString());

					Assert.IsTrue(enp, "没有正常进入线程池队列");
				}

				int workerThreads, completionPortThreads;
				ThreadPool.GetAvailableThreads(out  workerThreads, out completionPortThreads);

				Debug.WriteLine("线程池可用线程数" + workerThreads);

				while (count < 65536)
				{
					eventOne.WaitOne();
				}
			}

			watch.Stop();

			Console.WriteLine("结束，用时" + watch.ElapsedMilliseconds + "ms");
		}
		 */
	}
}
