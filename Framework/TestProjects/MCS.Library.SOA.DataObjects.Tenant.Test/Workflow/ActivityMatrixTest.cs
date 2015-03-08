using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class ActivityMatrixTest
    {
        [TestMethod]
        [Description("资源为活动矩阵时的，内嵌方法RowOperators.Count的测试")]
        public void ActiveMatrixRowOperatorsCountTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp();
            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);

            Assert.AreEqual(5, process.Activities.Count);
        }

        [TestMethod]
        [Description("资源为单行活动矩阵时的，与审批矩阵能匹配上的合并测试")]
        public void MergeMatchedOneActiveMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareOneRowActivityMatrixResourceDescriptor();

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareOneRowApprovalMatrixResourceDescriptor();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(approvalMatrix.PropertyDefinitions.Count - 1, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，与审批矩阵能匹配的合并测试")]
        public void MergeMatchedActiveMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            int originalCount = resource.Rows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(originalCount, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为单行活动矩阵时的，与审批矩阵能不能匹配的合并测试")]
        public void MergeNotMatchedOneActiveMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareOneRowActivityMatrixResourceDescriptor();

            int originalCount = resource.Rows.Count;

            resource.Rows[0].Values.FindByColumnName("ActivityCode").Value = "NotMatched";
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareOneRowApprovalMatrixResourceDescriptor();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(approvalMatrix.PropertyDefinitions.Count, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，与审批矩阵能不能匹配的合并测试")]
        public void MergeNotMatchedActiveMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            int originalCount = resource.Rows.Count;

            resource.Rows.ForEach(row => row.Values.FindByColumnName("ActivityCode").Value = "NotMatched");

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(8 + originalCount, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配的合并测试")]
        public void FilterAndMergeMatchedActiveMatrixAndApprovalMatrixTest()
        {
            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1001";

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            SOARolePropertyRowCollection activityRows = resource.Rows.Query(queryParam);

            int originalCount = activityRows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            activityRows.MergeToActivityMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            activityRows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(originalCount, activityRows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵不能匹配的合并测试")]
        public void FilterAndMergeNotMatchedActiveMatrixAndApprovalMatrixTest()
        {
            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1001";

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            resource.Rows.ForEach(row => row.Values.FindByColumnName("ActivityCode").Value = "NotMatched");

            SOARolePropertyRowCollection activityRows = resource.Rows.Query(queryParam);

            int originalActivityCount = activityRows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            int originalApprovalCount = approvalMatrix.PropertyDefinitions.Count - 1;

            activityRows.MergeToActivityMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            activityRows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(originalActivityCount + originalApprovalCount, activityRows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配，但是中间有角色的合并测试")]
        public void FilterAndMergeMatchedActiveMatrixWithRoleAndApprovalMatrixTest()
        {
            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1002";

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            SOARolePropertyRowCollection activityRows = resource.Rows.Query(queryParam);

            int originalCount = activityRows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            activityRows.MergeToActivityMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            activityRows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(3, activityRows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配的合并测试")]
        public void MergeMatchedActiveMatrixAndApprovalMatrixProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(approvalMatrix.ID);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);

            Assert.AreEqual(6, process.Activities.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵不能匹配的合并测试")]
        public void MergeNotMatchedActiveMatrixAndApprovalMatrixProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            resource.Rows.ForEach(row => row.Values.FindByColumnName("ActivityCode").Value = "NotMatched");

            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(resource, approvalMatrix.ID);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);

            process.Activities.ForEach(act =>
            {
                if (act.Descriptor.ActivityType == WfActivityType.NormalActivity)
                    if (act.Descriptor.Properties.GetValue("IsDynamic", false) == false)
                        Assert.AreEqual("审批", act.Descriptor.Name);
            });

            Assert.AreEqual(8, process.Activities.Count);
        }

        //[TestMethod]
        //[Description("资源为活动矩阵的动态序列化测试")]
        //public void ActiveMatrixResourceSerializationTest()
        //{
        //    WfActivityMatrixResourceDescriptor expected = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

        //    string serializedData = SerializationHelper.SerializeObjectToString(expected, SerializationFormatterType.Binary);

        //    WfActivityMatrixResourceDescriptor actual = SerializationHelper.DeserializeStringToObject<WfActivityMatrixResourceDescriptor>(serializedData, SerializationFormatterType.Binary);

        //    Assert.AreEqual(expected.PropertyDefinitions.Count, actual.PropertyDefinitions.Count);
        //    Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);

        //    Assert.AreEqual(expected.PropertyDefinitions.GetAllKeys().Count(), actual.PropertyDefinitions.GetAllKeys().Count());
        //}
    }
}
