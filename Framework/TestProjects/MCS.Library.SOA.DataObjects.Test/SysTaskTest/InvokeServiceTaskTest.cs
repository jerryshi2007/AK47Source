using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DO = MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.SysTaskTest
{
	[TestClass]
	public class InvokeServiceTaskTest
	{
		[TestMethod]
		[TestCategory("SysTask")]
		[Description("InvokeServiceTask的持久化测试")]
		public void InvokeServiceTaskPersistTest()
		{
			DO.InvokeServiceTask task = PrepareTask();

			DO.InvokeServiceTaskAdapter.Instance.Update(task);

			DO.InvokeServiceTask taskLoaded = (DO.InvokeServiceTask)DO.InvokeServiceTaskAdapter.Instance.Load(task.TaskID);

			Assert.IsNotNull(taskLoaded);
			Assert.IsTrue(taskLoaded.Data.IsNotEmpty());
			Assert.AreEqual(task.TaskID, taskLoaded.TaskID);
			Assert.AreEqual(task.TaskTitle, taskLoaded.TaskTitle);

			Assert.AreEqual(task.SvcOperationDefs.Count, taskLoaded.SvcOperationDefs.Count);
			Assert.AreEqual(task.SvcOperationDefs[0].AddressDef.Address, taskLoaded.SvcOperationDefs[0].AddressDef.Address);
			Assert.AreEqual(task.Context.Count, taskLoaded.Context.Count);
		}

		[TestMethod]
		[TestCategory("SysTask")]
		[Description("InvokeServiceTask执行测试")]
		public void InvokeServiceTaskExecuteTest()
		{
			DO.InvokeServiceTask task = PrepareTask();

			DO.InvokeServiceTaskAdapter.Instance.Update(task);

			DO.SysTask taskLoaded = DO.SysTaskAdapter.Instance.Load(task.TaskID);

			ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(taskLoaded.TaskType);

			executor.Execute(taskLoaded);

			Console.WriteLine(WfServiceInvoker.InvokeContext[task.SvcOperationDefs.FirstOrDefault().RtnXmlStoreParamName]);

			SysAccomplishedTask accomplishedTask = SysAccomplishedTaskAdapter.Instance.Load(taskLoaded.TaskID);

			Assert.IsNotNull(accomplishedTask);
			Assert.AreEqual(taskLoaded.Data, accomplishedTask.Data);

			Console.WriteLine(accomplishedTask.StatusText);
		}

		private static InvokeServiceTask PrepareTask()
		{
			string url = "http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx";

			DO.InvokeServiceTask task = new DO.InvokeServiceTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "新任务",
			};

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			parameters.Add(new WfServiceOperationParameter("callerID", "InvokeServiceTaskPersistTest"));

			task.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(
					new Workflow.WfServiceAddressDefinition(WfServiceRequestMethod.Post, null, url),
						"GetVersion", parameters, "ReturnValue")
				);

			return task;
		}
	}
}
