using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	[TestClass]
	public class JobTest
	{
		[TestInitialize]
		public void SetUp()
		{
		}

		[TestCleanup]
		public void TearDown()
		{

		}

		[TestMethod]
		[TestCategory("Job")]
		public void StartWorkflowJobTest()
		{
			TimeSpan offset = new TimeSpan(0, 6, 0);
			foreach (var job in CreateStartWorkflowJob())
			{
				job.LastExecuteTime = DateTime.Now.AddDays(-1);

				foreach (var sch in job.Schedules)
				{
					Console.WriteLine("任务描述{0}", sch.Description);
				}

				Console.WriteLine("上次执行时间{0}", job.LastExecuteTime);
				Console.WriteLine("job [{0}] try to running...", job.Name);
				//Console.WriteLine("下次执行时间{0}", job.NextExecuteTime);
				job.Start();
				//Console.WriteLine("下次执行时间{0}", job.NextExecuteTime);
				Console.WriteLine();
			}
		}

		[TestMethod]
		[TestCategory("Job")]
		public void InvokeWebServiceJobTest()
		{
			WfServiceInvoker.InvokeContext["callerID"] = UuidHelper.NewUuidString();

			JobBase job = CreateInvokeServiceJob();

			job.Schedules.Add(CreateSingleTimeSchedule());

			bool canStart = job.CanStart(TimeSpan.FromSeconds(60));

			Console.WriteLine("上次执行时间{0}", job.LastExecuteTime);
			Console.WriteLine("是否可以执行{0}", canStart);
			Console.WriteLine("job [{0}] try to running...", job.Name);
			//Console.WriteLine("下次执行时间{0}", job.NextExecuteTime);

			Assert.IsTrue(canStart);
			job.Start();

			string returnValue = WfServiceInvoker.InvokeContext.GetValueRecursively("Version", string.Empty);
			Console.WriteLine("Version: {0}", returnValue);

			Assert.IsTrue(returnValue.IndexOf(WfServiceInvoker.InvokeContext.GetValue("callerID", string.Empty)) >= 0);
		}

		[TestMethod]
		[TestCategory("Job")]
		[Description("根据Web Service相关的Job生成Task")]
		public void GenerateInvokeWebServiceJobTaskTest()
		{
			JobBaseAdapter.Instance.ClearAll();
			JobScheduleAdapter.Instance.ClearAll();
			SysTaskAdapter.Instance.ClearAll();

			JobSchedule schedule = CreateSingleTimeSchedule();
			JobScheduleAdapter.Instance.Update(schedule);

			for (int i = 0; i < 40; i++)
			{
				InvokeWebServiceJob job = (InvokeWebServiceJob)CreateInvokeServiceJob();
				job.Name = "即时调用服务" + i;
				job.Schedules.Add(schedule);

				InvokeWebServiceJobAdapter.Instance.Update(job);
			}

			SysTaskCollection sysTasks = new SysTaskCollection();

			List<Task> tasks = new List<Task>();

			for (int i = 0; i < 1; i++)
			{
				Task task = Task.Factory.StartNew(() =>
					JobBaseAdapter.Instance.FetchNotDispatchedJobsAndConvertToTask(40, TimeSpan.FromSeconds(60), (job, t) => sysTasks.Add(t)));

				tasks.Add(task);
			}

			Task.WaitAll(tasks.ToArray());

			Assert.AreEqual(40, sysTasks.Count);

			sysTasks.ForEach(st => Console.WriteLine(st.TaskTitle));
		}

		#region private
		private static JobCollection CreateStartWorkflowJob()
		{
			JobCollection jobs = new JobCollection();

			StartWorkflowJob sJob = new StartWorkflowJob();

			sJob.JobID = UuidHelper.NewUuidString();
			sJob.ProcessKey = "DefaultApprovalProcess";
			sJob.JobID = Guid.NewGuid().ToString();
			sJob.Name = "启动流程测试";
			sJob.Description = "小测一下";
			sJob.Creator = new OguUser("6872ac4c-48a2-47fc-a12f-05415dc50042"); //zhangyy
			sJob.Operator = new OguUser("22c3b351-a713-49f2-8f06-6b888a280fff"); //wangli5
			JobSchedule schedule = CreateSchedule();
			JobSchedule schedule0 = CreateSchedule();

			sJob.Schedules.Add(schedule);
			sJob.Schedules.Add(schedule0);

			jobs.Add(sJob);

			return jobs;
		}

		private static JobSchedule CreateSchedule()
		{
			string scheduleID = Guid.NewGuid().ToString();
			string scheduleName = "计划名称" + DateTime.Now.ToString();
			DateTime sTime = DateTime.Now.AddMonths(-1);
			DateTime eTime = DateTime.Now.AddMonths(1);

			TimeFrequencyBase timeFre = CreateTimeFrequency();
			JobScheduleFrequencyBase schFre = CreateSchFrequency(timeFre);

			JobSchedule schedule = new JobSchedule(scheduleID, scheduleName,
				sTime, eTime, schFre);

			return schedule;
		}

		private static JobScheduleFrequencyBase CreateSchFrequency(TimeFrequencyBase timeFre)
		{
			return new WeeklyJobScheduleFrequency(new List<DayOfWeek>() { DateTime.Now.DayOfWeek }, 1, timeFre);
		}

		private static TimeFrequencyBase CreateTimeFrequency()
		{
			return new RecurringTimeFrequency(5, IntervalUnit.Second, new TimeSpan(5, 0, 0), new TimeSpan(12, 0, 0));
		}

		private static JobSchedule CreateSingleTimeSchedule()
		{
			DateTime now = DateTime.Now;
			DateTime execTime = now.AddSeconds(6);

			FixedTimeFrequency timeFrequency = new FixedTimeFrequency(new TimeSpan(execTime.Hour, execTime.Minute, execTime.Second));
			DailyJobScheduleFrequency frequency = new DailyJobScheduleFrequency(1, timeFrequency);
			JobSchedule schedule = new JobSchedule(UuidHelper.NewUuidString(), "一般调度", now.AddDays(-1), frequency);

			schedule.SchduleType = JobSchduleType.Temporary;
			schedule.EndTime = now.AddHours(1);

			return schedule;
		}

		private static JobBase CreateInvokeServiceJob()
		{
			InvokeWebServiceJob job = new InvokeWebServiceJob();

			job.JobID = UuidHelper.NewUuidString();

			job.Name = "即时服务调用";
			job.SvcOperationDefs = new WfServiceOperationDefinitionCollection();
			job.LastStartExecuteTime = DateTime.Now.AddMinutes(-5);
			job.Category = "单元测试";

			WfServiceAddressDefinition address = new WfServiceAddressDefinition(WfServiceRequestMethod.Post,
				null,
				"http://localhost/MCSWebApp/PermissionCenterServices/Services/PermissionCenterToADService.asmx");

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			parameters.Add(new WfServiceOperationParameter() { Name = "callerID", Type = WfSvcOperationParameterType.RuntimeParameter, Value = "callerID" });

			WfServiceOperationDefinition serviceDef = new WfServiceOperationDefinition(address, "GetVersion", parameters, string.Empty);

			serviceDef.RtnXmlStoreParamName = "Version";
			serviceDef.TimeOut = 30000;
			job.SvcOperationDefs.Add(serviceDef);

			return job;
		}

		private static JobSchedule CreateImmediatelySchedule()
		{
			DateTime nextTime = DateTime.Now.AddSeconds(2);

			FixedTimeFrequency frequency = new FixedTimeFrequency(new TimeSpan(nextTime.Hour, nextTime.Minute, nextTime.Second));

			return new JobSchedule(UuidHelper.NewUuidString(), "立即执行的计划", DateTime.Now, null);
		}
		#endregion
	}
}
