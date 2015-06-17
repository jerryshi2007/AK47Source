using MCS.Library.Data.DataObjects;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class ActivityMatrixHelper
    {
        public static WfActivityMatrixResourceDescriptor PrepareEmptyActivityMatrixResourceDescriptorWithExternal()
        {
            WfActivityMatrixResourceDescriptor resource = PrepareActivityMatrixResourceDescriptor();

            WfApprovalMatrix externalMatrix = ApprovalMatrixHelper.PrepareApprovalMatrix();

            WfApprovalMatrixAdapter.Instance.Update(externalMatrix);

            resource.ExternalMatrixID = externalMatrix.ID;

            return resource;
        }

        public static WfActivityMatrixResourceDescriptor PrepareActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            resource.Rows.CopyFrom(PrepareRows(resource.PropertyDefinitions));

            return resource;
        }

        public static WfActivityMatrixResourceDescriptor PrepareSameSNActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            resource.Rows.CopyFrom(PrepareSameActivitySNRows(resource.PropertyDefinitions));

            return resource;
        }

        /// <summary>
        /// 准备一个一行的矩阵
        /// </summary>
        /// <returns></returns>
        public static WfActivityMatrixResourceDescriptor PrepareOneRowActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            resource.Rows.Add(PrepareOneRow(resource.PropertyDefinitions));

            return resource;
        }

        public static WfActivityMatrixResourceDescriptor PrepareReservedActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PrepareReservedPropertiesDefinition());
            resource.Rows.CopyFrom(PrepareReservedRows(resource.PropertyDefinitions));

            return resource;
        }

        /// <summary>
        /// 准备一个一行的内部包含动态资源的矩阵
        /// </summary>
        /// <returns></returns>
        public static WfActivityMatrixResourceDescriptor PrepareDynamicActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());

            resource.Rows.Add(PrepareOneDynamicRow(resource.PropertyDefinitions));
            resource.Rows.Add(PrepareTwoUsersDynamicRow(resource.PropertyDefinitions));
            resource.Rows.Add(PrepareInvalidUserDynamicRow(resource.PropertyDefinitions));

            return resource;
        }

        /// <summary>
        /// 创建一个都是预定义列的属性集合
        /// </summary>
        /// <returns></returns>
        public static SOARolePropertyDefinitionCollection PrepareReservedPropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivitySNColumn, SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ConditionColumn, SortOrder = 3, DefaultValue = "RowOperators.Count > 0" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivityCodeColumn, SortOrder = 4 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivityNameColumn, SortOrder = 6, DefaultValue = "审批" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.AutoExtractColumn, SortOrder = 6, DataType = ColumnDataType.Boolean, DefaultValue = "False" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.OperatorTypeColumn, SortOrder = 8, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.OperatorColumn, SortOrder = 9, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareReservedRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "fanhy" });


            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.Role, Operator = RolesDefineConfig.GetConfig().RolesDefineCollection["nestedRole"].Roles };

            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "20" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver2" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "Role" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = row3.Operator });

            SOARolePropertyRow row4 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User, Operator = "quym" };

            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "20" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver2" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "quym" });

            SOARolePropertyRow row5 = new SOARolePropertyRow() { RowNumber = 5, OperatorType = SOARoleOperatorType.User, Operator = "invalidUser" };

            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "30" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver3" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "invalidUser" });

            rows.Add(row1);
            rows.Add(row3);
            rows.Add(row4);
            rows.Add(row5);

            return rows;
        }

        public static SOARolePropertyDefinitionCollection PreparePropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivitySNColumn, SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "PayMethod", SortOrder = 2 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ConditionColumn, SortOrder = 3, DefaultValue = "RowOperators.Count > 0" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivityCodeColumn, SortOrder = 4 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.ActivityNameColumn, SortOrder = 6, DefaultValue = "审批" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.AutoExtractColumn, SortOrder = 6, DataType = ColumnDataType.Boolean, DefaultValue = "False" });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Age", SortOrder = 7, DataType = ColumnDataType.Integer });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.OperatorTypeColumn, SortOrder = 8, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = SOARolePropertyDefinition.OperatorColumn, SortOrder = 9, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static SOARolePropertyRow PrepareOneRow(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "一级审批" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "fanhy" });

            return row1;
        }

        private static SOARolePropertyRowCollection PrepareSameActivitySNRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "一级审批" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "fanhy" });

            rows.Add(row1);

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "quym" };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "一级审批" });
            row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "quym" });

            rows.Add(row2);

            return rows;
        }

        private static SOARolePropertyRow PrepareOneDynamicRow(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.Role, Operator = "ReportLine" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "一级审批" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.AutoExtractColumn]) { Value = "True" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "Role" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "ReportLine" });

            return row1;
        }

        private static SOARolePropertyRow PrepareTwoUsersDynamicRow(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "liming,liming,yangrui1" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "20" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "二级审批" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.AutoExtractColumn]) { Value = "True" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "liming,liming,yangrui1" });

            return row1;
        }

        private static SOARolePropertyRow PrepareInvalidUserDynamicRow(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "InvalidUser" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.AutoExtractColumn]) { Value = "True" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "InvalidUser" });

            return row1;
        }

        private static SOARolePropertyRowCollection PrepareRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "10" });
            //row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "一级审批" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row1.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "fanhy" });

            //SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "wangli5" };

            //row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            //row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            //row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "40" });
            //row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ColumnActivitySN]) { Value = "10" });
            ////row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            //row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver1" });
            //row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ColumnOperatorType]) { Value = "User" });
            //row2.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "wangli5" });

            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.Role, Operator = RolesDefineConfig.GetConfig().RolesDefineCollection["nestedRole"].Roles };

            row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row3.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row3.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "60" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "20" });
            //row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver2" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "二级审批" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "Role" });
            row3.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = row3.Operator });

            SOARolePropertyRow row4 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User, Operator = "quym" };

            row4.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row4.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row4.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "20" });
            //row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver2" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "二级审批" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row4.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "quym" });

            SOARolePropertyRow row5 = new SOARolePropertyRow() { RowNumber = 5, OperatorType = SOARoleOperatorType.User, Operator = "invalidUser" };

            row5.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row5.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row5.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = "30" });
            //row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ConditionColumn]) { Value = "RowOperators.Count > 0" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityCodeColumn]) { Value = "Approver3" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivityNameColumn]) { Value = "三级审批" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = "User" });
            row5.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = "invalidUser" });

            rows.Add(row1);
            //rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);
            rows.Add(row5);

            return rows;
        }
    }
}
