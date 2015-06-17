using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.DataObjects
{
    /// <summary>
    /// 测试数据实体对应的Adapter
    /// </summary>
    [TestClass]
    public class AdaptersTest
    {
        [TestMethod]
        [Description("应用程序的数据")]
        public void AppCommonInfoAdapterTest()
        {
            AppCommonInfoAdapter.Instance.ClearAll();
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
        }

        [TestMethod]
        [Description("资源可访问的ACL")]
        public void AclAdapterTest()
        {
            WfAclAdapter.Instance.ClearAll();

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
        }

        [TestMethod]
        [Description("流程流转时，查看同一资源的Assignee")]
        public void ProcessAclTest()
        {
            WfAclAdapter.Instance.ClearAll();

            IWfProcess process = ProcessHelper.CreateFreeStepsProcessDescriptor(
                OguObjectSettings.GetConfig().Objects["approver1"].User,
                OguObjectSettings.GetConfig().Objects["ceo"].User,
                OguObjectSettings.GetConfig().Objects["ceo"].User).StartupProcess();

            ((WfProcess)process).ResourceID = "resource2";

            process.MoveToDefaultActivityByExecutor();

            process = WfRuntime.GetProcessByProcessID(process.ID);
            process.MoveToDefaultActivityByExecutor();

            process = WfRuntime.GetProcessByProcessID(process.ID);
            process.MoveToDefaultActivityByExecutor();

            string userID = process.CurrentActivity.Assignees[0].User.ID;

            WfAclItemCollection currentItems = WfAclAdapter.Instance.Load(builder =>
            {
                builder.AppendItem("RESOURCE_ID", "resource2");
                builder.AppendItem("OBJECT_ID", userID);
            });

            Assert.AreEqual(1, currentItems.Count, "相同的人只保留一条记录");

            WfAclItemCollection allItems = WfAclAdapter.Instance.LoadByResourceID("resource2");
            Assert.AreEqual(2, allItems.Count, "只会有两条记录.在一个资源下同一个人只会有一条记录存在。但是如果存在委托时,则个数就不定");
        }
    }
}
