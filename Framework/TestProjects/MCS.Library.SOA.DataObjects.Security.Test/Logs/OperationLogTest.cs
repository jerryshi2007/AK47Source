using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Logs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.Logs
{
	[TestClass]
	public class OperationLogTest
	{
		[TestMethod]
		[TestCategory(Constants.OperationLogCategory)]
		[Description("操作日志入库的简单测试")]
		public void InsertLogTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCActionContext.Current.TimePoint = DateTime.Now.AddDays(-1).ToLocalTime();

			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			SCOperationLogAdapter.Instance.Insert(log);

			SCOperationLog logLoaded = SCOperationLogAdapter.Instance.Load(log.ID);

			Assert.IsNotNull(logLoaded);
			Assert.AreEqual(log.ID, logLoaded.ID);
			Assert.AreEqual(log.CorrelationID, logLoaded.CorrelationID);
			Assert.IsTrue(logLoaded.CreateTime.Subtract(SCActionContext.Current.TimePoint).TotalSeconds <= 1);
		}
	}
}
