using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Test.PersistQueue
{
    /// <summary>
    /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
    /// </summary>
	[TestClass]
	public class PersistQueueTest
	{
		[TestMethod]
		[Description("执行流程持久化队列中的操作测试")]
		public void PersistQueueBasicTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

			ProcessTestHelper.OutputExecutionTime(() => WfRuntime.PersistWorkflows(), "保存简单流程");

			WfPersistQueue pq = WfPersistQueue.FromProcess(process);

			Console.WriteLine(pq.ProcessID);

			Assert.AreEqual(0, WfProcessCurrentAssigneeAdapter.Instance.Load(pq.ProcessID).Count);
			Assert.AreEqual(0, WfProcessRelativeParamsAdapter.Instance.Load(pq.ProcessID).Count);
			Assert.IsNull(WfProcessDimensionAdapter.Instance.Load(pq.ProcessID));

			WfPersistQueueAdapter.Instance.DoQueueOperation(pq);

			Assert.IsTrue(WfProcessCurrentAssigneeAdapter.Instance.Load(pq.ProcessID).Count > 0);
			Assert.AreEqual(process.RelativeParams.Count, WfProcessRelativeParamsAdapter.Instance.Load(pq.ProcessID).Count);

			WfProcessDimension pd = WfProcessDimensionAdapter.Instance.Load(pq.ProcessID);

			Assert.IsNotNull(pd);

			Console.WriteLine(pd.Data);
		}

		[TestMethod]
		[Description("读取流程持久化队列中的操作测试")]
		public void FetchPersistQueueTest()
		{
			WfPersistQueueAdapter.Instance.ClearQueue();
			WfPersistQueueAdapter.Instance.ClearArchivedQueue();

			IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

			ProcessTestHelper.OutputExecutionTime(() => WfRuntime.PersistWorkflows(), "保存简单流程");

			Assert.AreEqual(0, WfProcessCurrentAssigneeAdapter.Instance.Load(process.ID).Count);
			Assert.AreEqual(0, WfProcessRelativeParamsAdapter.Instance.Load(process.ID).Count);
			Assert.IsNull(WfProcessDimensionAdapter.Instance.Load(process.ID));

			Console.WriteLine(process.ID);

			WfPersistQueue pq = WfPersistQueueAdapter.Instance.FetchQueueItemsAndDoOperation(1).FirstOrDefault();

			Assert.IsTrue(WfProcessCurrentAssigneeAdapter.Instance.Load(pq.ProcessID).Count > 0);
			Assert.AreEqual(process.RelativeParams.Count, WfProcessRelativeParamsAdapter.Instance.Load(pq.ProcessID).Count);

			Assert.IsNull(WfPersistQueueAdapter.Instance.Load(pq.ProcessID));

			WfPersistQueue pqArchieved = WfPersistQueueAdapter.Instance.LoadArchived(pq.ProcessID);

			Assert.IsNotNull(pqArchieved);
			Assert.AreEqual(pq.UpdateTag, pqArchieved.UpdateTag);

			WfProcessDimension pd = WfProcessDimensionAdapter.Instance.Load(pq.ProcessID);

			Assert.IsNotNull(pd);

			Console.WriteLine(pd.Data);
		}

		[TestMethod]
		[Description("流程信息变成持久化的XML信息的测试")]
		public void ProcessSimpleSerializationTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

			XElement processElem = XElement.Parse("<Process/>");
			((ISimpleXmlSerializer)process).ToXElement(processElem, string.Empty);

			Console.WriteLine(processElem.ToString());
		}
	}
}
