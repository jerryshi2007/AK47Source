using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.JobTest
{
	[TestClass]
	public class AdapterTest
	{
		[TestMethod]
		[TestCategory("Job")]
		public void JobBaseAdapterTest()
		{
			JobBase job = new StartWorkflowJob();

			job.JobID = Guid.NewGuid().ToString();
			job.Name = "NameTest" + DateTime.Now.ToString();
			job.Description = "DescTest" + DateTime.Now.ToString();
			job.Creator = new OguUser("6872ac4c-48a2-47fc-a12f-05415dc50042"); //张媛媛

			var schedule = CreateMonthlySchedule();
			JobScheduleAdapter.Instance.Update(schedule);

			job.Schedules.Add(schedule);
			JobBaseAdapter.Instance.Update(job);
			job.LastExecuteTime = DateTime.Now;
			JobBaseAdapter.Instance.Update(job);

			JobCollection coll = JobBaseAdapter.Instance.Load(p => p.AppendItem("JOB_ID", job.JobID));
			Assert.IsTrue(coll.Count == 1);

			Assert.AreEqual(job.Description, coll[0].Description);
			Assert.AreEqual(job.Schedules[0].Description, coll[0].Schedules[0].Description);

			JobBaseAdapter.Instance.Delete(job);
			coll = JobBaseAdapter.Instance.Load(p => p.AppendItem("JOB_ID", job.JobID));
			Assert.IsTrue(coll.Count == 0);
		}

		[TestMethod]
		[TestCategory("Job")]
		public void StartWorkflowJobAdapterTest()
		{
			StartWorkflowJob sJob = new StartWorkflowJob();
			Assert.IsTrue(sJob.Enabled);
			Assert.AreEqual(JobType.StartWorkflow, sJob.JobType);

			sJob.JobID = Guid.NewGuid().ToString();
			sJob.ProcessKey = "DefaultApprovalProcess";
			sJob.JobID = Guid.NewGuid().ToString();
			sJob.Name = "nametest20110407";
			sJob.Description = "desctest20110407";
			sJob.Creator = new OguUser("6872ac4c-48a2-47fc-a12f-05415dc50042"); //张媛媛

			var schedule = CreateMonthlySchedule();
			var schedule0 = CreateMonthlySchedule();
			JobScheduleAdapter.Instance.Update(schedule);
			JobScheduleAdapter.Instance.Update(schedule0);

			sJob.Schedules.Add(schedule);
			sJob.Schedules.Add(schedule0);
			StartWorkflowJobAdapter.Instance.Update(sJob);
			StartWorkflowJobAdapter.Instance.Update(sJob);

			StartWorkflowJobCollection coll = StartWorkflowJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", sJob.JobID));
			Assert.IsTrue(coll.Count == 1);

			Assert.AreEqual(sJob.Description, coll[0].Description);
			Assert.AreEqual(sJob.Schedules.Count, coll[0].Schedules.Count);
			Assert.AreEqual(sJob.Schedules[0].Description, coll[0].Schedules[0].Description);

			StartWorkflowJobAdapter.Instance.Delete(sJob);
			coll = StartWorkflowJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", sJob.JobID));
			Assert.IsTrue(coll.Count == 0);
		}

		[TestMethod]
		[TestCategory("Job")]
		public void JobScheduleAdapterTest()
		{
			JobSchedule schedule = CreateMonthlySchedule();

			JobScheduleAdapter.Instance.Update(schedule);

			var schedules = JobScheduleAdapter.Instance.Load(p => p.AppendItem("SCHEDULE_ID", schedule.ID));

			Assert.IsTrue(schedules.Count == 1);
			Assert.AreEqual(schedule.Description, schedules[0].Description);
			Console.WriteLine(schedule.Description);

			var undeletableIDs = JobScheduleAdapter.Instance.Delete(new string[] { schedule.ID });
			Assert.IsTrue(undeletableIDs.Length == 0);
		}

		private static JobSchedule CreateMonthlySchedule()
		{
			string schID = Guid.NewGuid().ToString();
			string schName = "TestName" + DateTime.Now.ToString();
			DateTime startTime = new DateTime(2011, 4, 1);
			DateTime endTime = new DateTime(2012, 4, 1);
			TimeFrequencyBase timeFrequency = new FixedTimeFrequency(new TimeSpan(9, 0, 0));

			JobScheduleFrequencyBase schFrequency = new MonthlyJobScheduleFrequency(10, 1, timeFrequency);
			schFrequency.ID = Guid.NewGuid().ToString();

			JobSchedule schedule = new JobSchedule(schID, schName, startTime, endTime, schFrequency);
			return schedule;
		}
	}
	/*
	[TestClass]
	public class InvokeWebServiceJobAdapterTest
	{
		[TestMethod]
		[TestCategory("Adapter")]
		[Description("Adapter")]
		public void InovkeWebServiceJobAdapter_Delete()
		{
			InvokeWebServiceJobAdapter.Instance.Delete(new string[] { "dca1c503-429f-4879-b866-8cec98bd16cc" });
		}
	} */
}
