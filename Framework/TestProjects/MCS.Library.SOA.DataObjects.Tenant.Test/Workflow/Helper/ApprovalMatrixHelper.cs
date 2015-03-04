using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class ApprovalMatrixHelper
    {
        public static WfApprovalMatrix PrepareApprovalMatrix()
        {
            WfApprovalMatrix matrix = new WfApprovalMatrix() { ID = UuidHelper.NewUuidString() };

            matrix.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            matrix.Rows.CopyFrom(PrepareRows(matrix.PropertyDefinitions));

            return matrix;
        }

        private static SOARolePropertyDefinitionCollection PreparePropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Approver1", SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Approver2", SortOrder = 2 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Approver3", SortOrder = 3 });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = "fanhy" });
            row1.Values.Add(new SOARolePropertyValue(pds["Approver2"]) { Value = "quym" });
            row1.Values.Add(new SOARolePropertyValue(pds["Approver3"]) { Value = "liming" });

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row2.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = string.Empty });
            row2.Values.Add(new SOARolePropertyValue(pds["Approver2"]) { Value = "quym" });
            row2.Values.Add(new SOARolePropertyValue(pds["Approver3"]) { Value = "liming" });

            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.User };

            row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1003" });
            row3.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = string.Empty });
            row3.Values.Add(new SOARolePropertyValue(pds["Approver2"]) { Value = string.Empty });
            row3.Values.Add(new SOARolePropertyValue(pds["Approver3"]) { Value = "liming" });

            SOARolePropertyRow row4 = new SOARolePropertyRow() { RowNumber = 4, OperatorType = SOARoleOperatorType.User };

            row4.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1003" });
            row4.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = "fanhy" });
            row4.Values.Add(new SOARolePropertyValue(pds["Approver2"]) { Value = string.Empty });
            row4.Values.Add(new SOARolePropertyValue(pds["Approver3"]) { Value = "liming" });

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);

            return rows;
        }
    }
}
