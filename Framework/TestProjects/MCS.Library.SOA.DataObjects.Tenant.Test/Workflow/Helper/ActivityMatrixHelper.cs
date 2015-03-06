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

        private static SOARolePropertyDefinitionCollection PreparePropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "ActivitySN", SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "PayMethod", SortOrder = 2 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Condition", SortOrder = 3 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Age", SortOrder = 2, DataType = ColumnDataType.Integer });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row1.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "10" });
            row1.Values.Add(new SOARolePropertyValue(pds["Condition"]) { Value = "RowOperators.Count > 0" });

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "wangli5" };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "40" });
            row2.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "10" });
            row2.Values.Add(new SOARolePropertyValue(pds["Condition"]) { Value = "RowOperators.Count > 0" });

            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.Role, Operator = RolesDefineConfig.GetConfig().RolesDefineCollection["nestedRole"].Roles };

            row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row3.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row3.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "60" });
            row3.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "20" });
            row3.Values.Add(new SOARolePropertyValue(pds["Condition"]) { Value = "RowOperators.Count > 0" });

            SOARolePropertyRow row4 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User, Operator = "quym" };

            row4.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row4.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row4.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row4.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "20" });
            row4.Values.Add(new SOARolePropertyValue(pds["Condition"]) { Value = "RowOperators.Count > 0" });

            SOARolePropertyRow row5 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User, Operator = "invalidUser" };

            row5.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row5.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row5.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });
            row5.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = "30" });
            row5.Values.Add(new SOARolePropertyValue(pds["Condition"]) { Value = "RowOperators.Count > 0" });

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);
            rows.Add(row5);

            return rows;
        }
    }
}
