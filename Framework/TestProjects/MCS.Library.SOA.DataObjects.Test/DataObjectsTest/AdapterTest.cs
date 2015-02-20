using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Test
{
	[TestClass]
	public class AdapterTest
	{
		[TestMethod]
		[Description("应用程序的数据")]
		[TestCategory(ProcessTestHelper.Data)]
		public void AppCommonInfoAdapterTest()
		{
			AppCommonInfo info = new AppCommonInfo();
			info.ResourceID = "resource1";
			info.Subject = "测试";
			info.CreateTime = DateTime.Now;
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			info.Creator = user;

			AppCommonInfoAdapter.Instance.Update(info);
			AppCommonInfo commonInfo = AppCommonInfoAdapter.Instance.Load("resource1");

			AppCommonInfoCollection coll = AppCommonInfoAdapter.Instance.Load(builder => builder.AppendItem("RESOURCE_ID", "resource1"));

			Assert.AreEqual(commonInfo.ResourceID, coll[0].ResourceID);

			AppCommonInfoAdapter.Instance.Delete(info);
		}

		[TestMethod]
		[Description("资源可访问的ACL")]
		[TestCategory(ProcessTestHelper.Data)]
		public void WfAclAdapterTest()
		{
			WfAclItem item = new WfAclItem();
			item.ResourceID = "resource1";
			item.ObjectID = "object1";
			item.ObjectType = "dept";
			item.Source = "workfolw";

			WfAclAdapter.Instance.Update(item);

			WfAclItemCollection coll = WfAclAdapter.Instance.Load(builder => builder.AppendItem("RESOURCE_ID", "resource1"));
			Assert.IsTrue(coll.Count > 0);

			WfAclItemCollection itemColl = WfAclAdapter.Instance.LoadByResourceID("resource1");
			Assert.IsTrue(coll.Count > 0);

			Assert.AreEqual(coll[0].ResourceID, itemColl[0].ResourceID);
			Assert.AreEqual(coll[0].ObjectID, itemColl[0].ObjectID);

			WfAclAdapter.Instance.Delete(item);
		}

		[TestMethod]
		[Description("流程流转时，查看同一资源的Assignee")]
		[TestCategory(ProcessTestHelper.Data)]
		public void WfAclAdapterDataTest()
		{

			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
			((WfProcess)process).ResourceID = "resource2";

			ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.requestor, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, process);

			string userId = process.CurrentActivity.Assignees[0].User.ID;

			WfAclItemCollection itemColl = WfAclAdapter.Instance.Load(builder =>
			{
				builder.AppendItem("RESOURCE_ID", "resource2");
				builder.AppendItem("OBJECT_ID", userId);
			});

			Assert.AreEqual(1, itemColl.Count, "相同的人只保留一条记录");

			WfAclItemCollection coll = WfAclAdapter.Instance.LoadByResourceID("resource2");
			Assert.AreEqual(2, coll.Count,"只会有两条记录.在一个资源下同一个人只会有一条记录存在.但是如果存在委托时,则个数就不定"); 

			foreach (WfAclItem data in coll)
			{
				WfAclAdapter.Instance.Delete(data);
			}
		}

		[TestMethod]
		[Description("用户操作日志")]
		[TestCategory(ProcessTestHelper.Data)]
		public void UserOperationLogAdapterTest()
		{
			int resId = 3;
			int actId = 3;
			UserOperationLog log = GetInstanceOfUserOperationLog(resId, actId);

			UserOperationLogAdapter.Instance.Update(log);


			UserOperationLogCollection log3 = UserOperationLogAdapter.Instance.LoadByResourceID("resource" + resId.ToString());

			UserOperationLog log2 = UserOperationLogAdapter.Instance.Load(log3[0].ID);

			Assert.AreEqual(log.ResourceID, log2.ResourceID);
			Assert.AreEqual(log.Operator.ID, log2.Operator.ID);

			UserOperationLogAdapter.Instance.Delete(log);
		}

		private static UserOperationLog GetInstanceOfUserOperationLog(int resId, int actId)
		{
			UserOperationLog log = new UserOperationLog();
			log.ResourceID = "resource" + resId.ToString();
			log.Subject = "subjectName";
			log.ApplicationName = "Seagull";
			log.ProgramName = "program1";
			log.ProcessID = "process1";
			log.ActivityID = "activityId" + actId.ToString();
			log.ActivityName = "add new activity";

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			log.Operator = user;
			log.TopDepartment = user.Parent;
			log.OperationDateTime = DateTime.Now;

			log.RealUser = user;
			log.OperationName = "zl";
			log.OperationType = OperationType.Add;
			log.OperationDescription = "add data";
			return log;
		}
	}
}
