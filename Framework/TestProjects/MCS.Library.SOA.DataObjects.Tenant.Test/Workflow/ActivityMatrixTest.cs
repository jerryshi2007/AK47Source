using MCS.Library.Core;
using MCS.Library.OGUPermission;
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
        public void ActivityMatrixRowOperatorsCountTest()
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
        [Description("资源为活动矩阵时的，内部的活动包含动态活动")]
        public void ActivityMatrixWithDynamicActivityTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareDynamicActivityMatrixResourceDescriptor();
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(resource, string.Empty);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 },
                    { "ReportLine", new IUser[] {OguObjectSettings.GetConfig().Objects["approver1"].User, OguObjectSettings.GetConfig().Objects["cfo"].User}}
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);

            Assert.AreEqual(7, process.Activities.Count);
        }

        [TestMethod]
        [Description("资源为活动矩阵时的，内部的活动包含动态活动，然后再合并审批矩阵的测试")]
        public void MergeActivityMatrixWithDynamicActivityAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareDynamicActivityMatrixResourceDescriptor();

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp(resource, approvalMatrix.ID);

            IWfProcess process = ProcessHelper.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 },
                    { "ReportLine", new IUser[] {OguObjectSettings.GetConfig().Objects["approver1"].User, OguObjectSettings.GetConfig().Objects["cfo"].User}}
				});

            Console.WriteLine(process.Activities.Count);

            WfOutputHelper.OutputMainStream(process);
            WfOutputHelper.OutputEveryActivities(process);
        }

        [TestMethod]
        [Description("资源为单行活动矩阵时的，与审批矩阵能匹配上的合并测试")]
        public void MergeMatchedOneActivityMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareOneRowActivityMatrixResourceDescriptor();

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareOneRowApprovalMatrixResourceDescriptor();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(approvalMatrix.PropertyDefinitions.Count - 1, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，与审批矩阵能匹配的合并测试")]
        public void MergeMatchedActivityMatrixAndApprovalMatrixTest()
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
        public void MergeNotMatchedOneActivityMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareOneRowActivityMatrixResourceDescriptor();

            int originalCount = resource.Rows.Count;

            resource.Rows[0].Values.FindByColumnName(SOARolePropertyDefinition.ActivityCodeColumn).Value = "NotMatched";
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareOneRowApprovalMatrixResourceDescriptor();

            resource.MergeApprovalMatrix(approvalMatrix);

            Assert.AreEqual(approvalMatrix.PropertyDefinitions.Count, resource.Rows.Count);

            resource.AssertAndOutputMatrixOperators();
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，与审批矩阵能不能匹配的合并测试")]
        public void MergeNotMatchedActivityMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            int originalCount = resource.Rows.Count;

            resource.Rows.ForEach(row => row.Values.FindByColumnName(SOARolePropertyDefinition.ActivityCodeColumn).Value = "NotMatched");

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            resource.MergeApprovalMatrix(approvalMatrix);

            resource.AssertAndOutputMatrixOperators();

            Assert.AreEqual(8 + originalCount, resource.Rows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配的合并测试")]
        public void FilterAndMergeMatchedActivityMatrixAndApprovalMatrixTest()
        {
            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1001";

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            SOARolePropertyRowCollection activityRows = resource.Rows.Query(queryParam);

            int originalCount = activityRows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            activityRows.MergeApprovalMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            activityRows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(originalCount, activityRows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵不能匹配的合并测试")]
        public void FilterAndMergeNotMatchedActivityMatrixAndApprovalMatrixTest()
        {
            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1001";

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            resource.Rows.ForEach(row => row.Values.FindByColumnName(SOARolePropertyDefinition.ActivityCodeColumn).Value = "NotMatched");

            SOARolePropertyRowCollection activityRows = resource.Rows.Query(queryParam);

            int originalActivityCount = activityRows.Count;

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            int originalApprovalCount = approvalMatrix.PropertyDefinitions.Count - 1;

            activityRows.MergeApprovalMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

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

            activityRows.MergeApprovalMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            activityRows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(3, activityRows.Count);
        }

        [TestMethod]
        [Description("当活动矩阵行列为空时，与过滤后的审批矩阵匹配的合并测试")]
        public void FilterAndMergeEmptyActivityMatrixAndApprovalMatrixTest()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

            queryParam.QueryName = "CostCenter";
            queryParam.QueryValue = "1001";

            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            SOARolePropertyRowCollection approvalRows = approvalMatrix.Rows.Query(queryParam);

            resource.Rows.MergeApprovalMatrix(resource.PropertyDefinitions, approvalRows, approvalMatrix.PropertyDefinitions);

            //输出的只有RowNumber。因为活动矩阵没有列
            resource.Rows.AssertAndOutputMatrixOperators();

            Assert.AreEqual(3, resource.Rows.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配的合并测试")]
        public void MergeMatchedActivityMatrixAndApprovalMatrixProcessTest()
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
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵能匹配的合并测试，审批矩阵的列和活动矩阵的行不匹配")]
        public void MergeMatchedActivityMatrixAndExtraApprovalMatrixProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareExtrApprovalMatrix();

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

            //有一个非法用户，因此去掉一个环节
            Assert.AreEqual(6, process.Activities.Count);
        }

        [TestMethod]
        [Description("资源为多行活动矩阵时的，过滤后与审批矩阵不能匹配的合并测试")]
        public void MergeNotMatchedActivityMatrixAndApprovalMatrixProcessTest()
        {
            WfApprovalMatrix approvalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(approvalMatrix);

            WfActivityMatrixResourceDescriptor resource = ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor();

            resource.Rows.ForEach(row => row.Values.FindByColumnName(SOARolePropertyDefinition.ActivityCodeColumn).Value = "NotMatched");

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
                    {
                        Console.WriteLine(act.Descriptor.Name);
                        Assert.IsTrue(act.Descriptor.Name.IndexOf("审批") >= 0);
                    }
            });

            Assert.AreEqual(8, process.Activities.Count);
        }
    }
}
