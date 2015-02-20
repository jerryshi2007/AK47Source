using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using System.Xml.Linq;
using MCS.Library.Expression;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
    [TestClass]
    public class WfProcessDescriptorInfoTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        [Description("简单的流程描述测试")]
        [TestCategory(ProcessTestHelper.Descriptor)]
        public void SimpleProcessDescriptorTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            Console.WriteLine("Init Act Key: {0}, Completed Act Key: {1}, Transition Key: {2}",
                processDesp.InitialActivity.Key, processDesp.CompletedActivity.Key, processDesp.InitialActivity.ToTransitions[0].Key);

            Assert.IsTrue(processDesp.InitialActivity.CanReachTo(processDesp.CompletedActivity));
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Data)]
        public void UpdateWfProcessDescriptorInfoTest()
        {
            WfProcessDescriptorInfo processInfo = PrepareProcessDescriptorInfo();

            WfProcessDescriptorInfoAdapter.Instance.Update(processInfo);

            WfProcessDescriptorInfo loadedProcessInfo = WfProcessDescriptorInfoAdapter.Instance.Load(processInfo.ProcessKey);

            try
            {
                Assert.AreEqual(processInfo.ProcessKey, loadedProcessInfo.ProcessKey);
                Assert.AreEqual(processInfo.ApplicationName, loadedProcessInfo.ApplicationName);
                Assert.AreEqual(processInfo.ProgramName, loadedProcessInfo.ProgramName);
                Assert.AreEqual(processInfo.ProcessName, loadedProcessInfo.ProcessName);

                XElement root = XElement.Parse(loadedProcessInfo.Data);

                Assert.AreEqual(processInfo.Data, root.ToString(), "字符串的格式不同");

                Assert.AreEqual(processInfo.Creator.ID, loadedProcessInfo.Creator.ID);
                Assert.AreEqual(processInfo.Creator.Name, loadedProcessInfo.Creator.Name);

                //Assert.AreEqual(processInfo.Modifier.ID, loadedProcessInfo.Modifier.ID);
                //Assert.AreEqual(processInfo.Modifier.Name, loadedProcessInfo.Modifier.Name);

                Assert.IsTrue(loadedProcessInfo.CreateTime != DateTime.MinValue);
                //Assert.IsTrue(loadedProcessInfo.ModifyTime != DateTime.MinValue);
            }
            finally
            {
                WfProcessDescriptorInfoAdapter.Instance.Delete(loadedProcessInfo);
            }
        }

        [TestMethod]
        public void UpdateWfProcessDescriptorInfoWithTenantCodeTest()
        {
            TenantContext.Current.TenantCode = "1001";

            WfProcessDescriptorInfo processInfo = PrepareProcessDescriptorInfo();

            WfProcessDescriptorInfoAdapter.Instance.Update(processInfo);
        }

        private static WfProcessDescriptorInfo PrepareProcessDescriptorInfo()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            WfProcessDescriptorInfo processInfo = WfProcessDescriptorInfo.FromProcessDescriptor(processDesp);

            processInfo.Creator = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
            processInfo.Modifier = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

            return processInfo;
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Descriptor)]
        public void CreateProcessDescriptorWithBranchProcDesp()
        {
            //分支流程描述
            IWfProcessDescriptor procDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            //主流程描述
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptor();

            WfBranchProcessTemplateDescriptor data = new WfBranchProcessTemplateDescriptor("branchProcessTemplateDesp");
            data.BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;
            data.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;
            data.BranchProcessKey = procDesp.Key;

            processDesp.Activities[1].BranchProcessTemplates.Add(data);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Descriptor)]
        public void WfDynamicResourceDescriptorTest()
        {
            WfRuntime.ProcessContext.EvaluateDynamicResourceCondition += new Expression.CalculateUserFunction(ProcessContext_EvaluateDynamicResourceCondition);

            WfResourceDescriptorCollection resources = new WfResourceDescriptorCollection();

            WfDynamicResourceDescriptor resource = new WfDynamicResourceDescriptor();

            resource.Condition.Expression = "Requestor";

            resources.Add(resource);

            OguDataCollection<IUser> users = resources.ToUsers();

            users.ForEach(u => Console.WriteLine(u.DisplayName));
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Descriptor)]
        [Description("测试空对象的动态资源")]
        public void WfEmptyUserDynamicResourceDescriptorTest()
        {
            WfRuntime.ProcessContext.EvaluateDynamicResourceCondition += new Expression.CalculateUserFunction(ProcessContext_EvaluateDynamicResourceCondition);

            WfResourceDescriptorCollection resources = new WfResourceDescriptorCollection();

            WfDynamicResourceDescriptor resource = new WfDynamicResourceDescriptor();

            resource.Condition.Expression = "EmptyUser";

            resources.Add(resource);

            OguDataCollection<IUser> users = resources.ToUsers();

            Assert.AreEqual(0, users.Count);
        }

        private object ProcessContext_EvaluateDynamicResourceCondition(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            object result = null;

            switch (funcName)
            {
                case "Requestor":
                    List<IUser> users = new List<IUser>();

                    users.Add((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object);
                    users.Add((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

                    result = users;
                    break;
                case "EmptyUser":
                    OguUser user = new OguUser("FakeID");
                    user.ID = string.Empty;
                    result = user;
                    break;
            }

            return result;
        }
    }
}
