using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.JsonSerializationTest
{
	/// <summary>
	/// 分支流程启动参数的JSON序列化测试
	/// </summary>
	[TestClass]
	public class BranchParamsJsonTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试分支流程启动参数的JSON序列化测试")]
		public void BranchProcessStartupParamsJsonTest()
		{
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object;
			WfBranchProcessStartupParams data = new WfBranchProcessStartupParams(user);

			data.Department = user.Parent;
			data.RelativeParams["RP"] = UuidHelper.NewUuidString();
			data.ApplicationRuntimeParameters["context"] = UuidHelper.NewUuidString();
			data.ResourceID = UuidHelper.NewUuidString();
			data.DefaultTaskTitle = UuidHelper.NewUuidString();
			data.StartupContext = UuidHelper.NewUuidString();

			WfConverterHelper.RegisterConverters();

			string serilizedData = JSONSerializerExecute.Serialize(data);

			Console.WriteLine(serilizedData);

			WfBranchProcessStartupParams deserilizedData = JSONSerializerExecute.Deserialize<WfBranchProcessStartupParams>(serilizedData);

			AssertBranchProcessStartupParams(data, deserilizedData);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试分支流转参数的JSON序列化测试")]
		public void WfBranchProcessTransferParamsJsonTest()
		{
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object;

			IWfBranchProcessTemplateDescriptor template = WfProcessTestCommon.CreateTemplate("Consign", user);

			WfBranchProcessTransferParams data = new WfBranchProcessTransferParams(template);

			data.BranchParams.Clear();

			WfBranchProcessStartupParams bpsp = new WfBranchProcessStartupParams(user);

			bpsp.Department = user.Parent;
			bpsp.RelativeParams["RP"] = UuidHelper.NewUuidString();
			bpsp.ApplicationRuntimeParameters["context"] = UuidHelper.NewUuidString();
			bpsp.ResourceID = UuidHelper.NewUuidString();
			bpsp.DefaultTaskTitle = UuidHelper.NewUuidString();
			bpsp.StartupContext = UuidHelper.NewUuidString();

			data.BranchParams.Add(bpsp);

			string serilizedData = JSONSerializerExecute.Serialize(data);

			Console.WriteLine(serilizedData);

			WfBranchProcessTransferParams deserilizedData = JSONSerializerExecute.Deserialize<WfBranchProcessTransferParams>(serilizedData);

			Assert.AreEqual(data.Template.Key, deserilizedData.Template.Key);
			Assert.AreEqual(data.Template.BranchProcessKey, deserilizedData.Template.BranchProcessKey);

			AssertBranchProcessStartupParams(data.BranchParams[0], deserilizedData.BranchParams[0]);
		}

		private static void AssertBranchProcessStartupParams(WfBranchProcessStartupParams data, WfBranchProcessStartupParams deserilizedData)
		{
			Assert.AreEqual(data.ResourceID, deserilizedData.ResourceID);
			Assert.AreEqual(data.DefaultTaskTitle, deserilizedData.DefaultTaskTitle);
			Assert.AreEqual(data.StartupContext, deserilizedData.StartupContext);

			Assert.AreEqual(data.ApplicationRuntimeParameters["context"], deserilizedData.ApplicationRuntimeParameters["context"]);
			Assert.AreEqual(data.RelativeParams["RP"], deserilizedData.RelativeParams["RP"]);

			Assert.AreEqual(data.Department.ID, deserilizedData.Department.ID);

			Assert.AreEqual(data.Assignees[0].User.ID, deserilizedData.Assignees[0].User.ID);
		}
	}
}
