using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Common.Test
{
    public static class ProcessDescriptorHelper
    {
        /// <summary>
        /// 创建一个没有连线的客户端流程对象
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor CreateSimpleClientProcessWithoutLines()
        {
            WfClientProcessDescriptor processDesp = new WfClientProcessDescriptor();

            processDesp.Key = UuidHelper.NewUuidString();
            processDesp.Name = "客户端测试流程";

            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleClientActivityWithUser("Start", "开始", "Requestor", WfClientActivityType.InitialActivity));
            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleClientActivity("End", "结束", WfClientActivityType.CompletedActivity));

            processDesp.RelativeLinks.Add(new WfClientRelativeLinkDescriptor("AP1") { Category = "Process", Url = "http://www.ak47.com" });
            processDesp.CancelEventReceivers.Add(new WfClientUserResourceDescriptor(Consts.Users["Requestor"]));

            return processDesp;
        }

        /// <summary>
        /// 创建一个有连线的流程
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor CreateSimpleClientProcessWithLines()
        {
            WfClientProcessDescriptor processDesp = CreateSimpleClientProcessWithoutLines();

            WfClientTransitionDescriptor transition = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, processDesp.CompletedActivity.Key);

            transition.Key = "L1";

            processDesp.InitialActivity.ToTransitions.Add(transition);

            return processDesp;
        }

        /// <summary>
        /// 创建一个有4个节点的流程，开始点有两条出线，根据Amount是否大于等于5000来判断
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor CreateClientProcessWithConditionLines()
        {
            WfClientProcessDescriptor processDesp = CreateSimpleClientProcessWithoutLines();

            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleClientActivityWithUser("N1", "CFO", "CFO", WfClientActivityType.NormalActivity));
            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleClientActivityWithUser("N2", "CEO", "CEO", WfClientActivityType.NormalActivity));

            WfClientTransitionDescriptor transitionToCFO = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, "N1");

            transitionToCFO.Key = "L1";
            transitionToCFO.Condition.Expression = "Amount >= 5000";
            processDesp.InitialActivity.ToTransitions.Add(transitionToCFO);

            WfClientTransitionDescriptor transitionToCEO = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, "N2");

            transitionToCEO.Key = "L2";
            transitionToCEO.Condition.Expression = "Amount < 5000";

            processDesp.InitialActivity.ToTransitions.Add(transitionToCEO);

            WfClientTransitionDescriptor transitionCEOToCFO = new WfClientTransitionDescriptor("CFO", "CEO");

            transitionCEOToCFO.Key = "L3";

            processDesp.Activities["N1"].ToTransitions.Add(transitionCEOToCFO);

            return processDesp;
        }

        /// <summary>
        /// 创建一个带动态矩阵资源的流程定义
        /// </summary>
        /// <returns></returns>
        public static WfClientProcessDescriptor CreateClientProcessWithActivityMatrixResourceDescriptor()
        {
            WfClientProcessDescriptor processDesp = CreateSimpleClientProcessWithoutLines();

            WfClientActivityDescriptor actDesp = ActivityDescriptorHelper.CreateSimpleClientActivity("N1", "活动矩阵", WfClientActivityType.NormalActivity);

            actDesp.Properties.AddOrSetValue("IsDynamic", true);
            actDesp.Resources.Add(GetClientActivityMatrixResourceDescriptor());

            processDesp.Activities.Add(actDesp);

            WfClientTransitionDescriptor transitionToN1 = new WfClientTransitionDescriptor(processDesp.InitialActivity.Key, "N1") { Key = "L1" };

            processDesp.InitialActivity.ToTransitions.Add(transitionToN1);

            WfClientTransitionDescriptor transitionToCompleted = new WfClientTransitionDescriptor(actDesp.Key, processDesp.CompletedActivity.Key) { Key = "L2" };

            processDesp.Activities["N1"].ToTransitions.Add(transitionToCompleted);

            return processDesp;
        }

        public static WfCreateClientDynamicProcessParams CreateClientDynamicProcessParams()
        {
            WfCreateClientDynamicProcessParams createParams = new WfCreateClientDynamicProcessParams();

            createParams.Key = UuidHelper.NewUuidString();
            createParams.Name = "客户端测试流程参数";

            createParams.ActivityMatrix = GetClientActivityMatrixResourceDescriptor();

            return createParams;
        }

        /// <summary>
        /// 创建一个没有连线的Server端流程对象
        /// </summary>
        /// <returns></returns>
        public static WfProcessDescriptor CreateSimpleServerProcessWithoutLines()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = UuidHelper.NewUuidString();
            processDesp.Name = "服务端测试流程";

            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleServerActivity("Start", "开始", WfActivityType.InitialActivity));
            processDesp.Activities.Add(ActivityDescriptorHelper.CreateSimpleServerActivity("End", "结束", WfActivityType.CompletedActivity));

            processDesp.RelativeLinks.Add(new WfRelativeLinkDescriptor("AR1") { Category = "Activity", Url = "http://www.ak47.com" });

            return processDesp;
        }

        /// <summary>
        /// 创建一个有连线的Server端流程
        /// </summary>
        /// <returns></returns>
        public static WfProcessDescriptor CreateSimpleServerProcessWithLines()
        {
            WfProcessDescriptor processDesp = CreateSimpleServerProcessWithoutLines();

            WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor("L1");

            transition.ConnectActivities(processDesp.InitialActivity, processDesp.CompletedActivity);

            return processDesp;
        }

        #region WfActivityMatrixResourceDescriptor
        public static WfActivityMatrixResourceDescriptor GetServerActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.ExternalMatrixID = UuidHelper.NewUuidString();

            resource.PropertyDefinitions.CopyFrom(PrepareServerPropertiesDefinition());
            resource.Rows.CopyFrom(PrepareServerRows(resource.PropertyDefinitions));

            return resource;
        }

        private static SOARolePropertyDefinitionCollection PrepareServerPropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "PayMethod", SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Age", SortOrder = 2, DataType = ColumnDataType.Integer });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareServerRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "wangli5" };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "40" });

            rows.Add(row1);
            rows.Add(row2);

            return rows;
        }
        #endregion WfActivityMatrixResourceDescriptor

        #region WfClientActivityMatrixResourceDescriptor
        public static WfClientActivityMatrixResourceDescriptor GetClientActivityMatrixResourceDescriptor()
        {
            WfClientActivityMatrixResourceDescriptor resource = new WfClientActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PrepareClientPropertiesDefinition());
            resource.Rows.CopyFrom(PrepareClientRows(resource.PropertyDefinitions));

            return resource;
        }

        private static WfClientRolePropertyDefinitionCollection PrepareClientPropertiesDefinition()
        {
            WfClientRolePropertyDefinitionCollection propertiesDefinition = new WfClientRolePropertyDefinitionCollection();

            propertiesDefinition.Add(new WfClientRolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition() { Name = "PayMethod", SortOrder = 1 });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition() { Name = "Age", SortOrder = 2, DataType = ColumnDataType.Integer });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition() { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new WfClientRolePropertyDefinition() { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static WfClientRolePropertyRowCollection PrepareClientRows(WfClientRolePropertyDefinitionCollection pds)
        {
            WfClientRolePropertyRowCollection rows = new WfClientRolePropertyRowCollection();

            WfClientRolePropertyRow row1 = new WfClientRolePropertyRow() { RowNumber = 1, OperatorType = WfClientRoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new WfClientRolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new WfClientRolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new WfClientRolePropertyValue(pds["Age"]) { Value = "30" });

            WfClientRolePropertyRow row2 = new WfClientRolePropertyRow() { RowNumber = 2, OperatorType = WfClientRoleOperatorType.User, Operator = "wangli5" };

            row2.Values.Add(new WfClientRolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row2.Values.Add(new WfClientRolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row2.Values.Add(new WfClientRolePropertyValue(pds["Age"]) { Value = "40" });

            rows.Add(row1);
            rows.Add(row2);

            return rows;
        }
        #endregion WfClientActivityMatrixResourceDescriptor
    }
}
