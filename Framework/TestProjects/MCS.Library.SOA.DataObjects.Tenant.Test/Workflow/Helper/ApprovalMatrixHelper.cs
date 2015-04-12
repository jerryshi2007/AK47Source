﻿using MCS.Library.Core;
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

        /// <summary>
        /// 创建一个与活动矩阵不匹配列的审批矩阵
        /// </summary>
        /// <returns></returns>
        public static WfApprovalMatrix PrepareExtrApprovalMatrix()
        {
            WfApprovalMatrix matrix = new WfApprovalMatrix() { ID = UuidHelper.NewUuidString() };

            matrix.PropertyDefinitions.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            matrix.PropertyDefinitions.Add(new SOARolePropertyDefinition() { Name = "ExtraApprover", SortOrder = 1 });

            SOARolePropertyDefinitionCollection pds = matrix.PropertyDefinitions;

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = string.Empty };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["ExtraApprover"]) { Value = "wangli5" });

            matrix.Rows.Add(row1);

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = string.Empty };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row2.Values.Add(new SOARolePropertyValue(pds["ExtraApprover"]) { Value = "InvalidUser" });

            matrix.Rows.Add(row2);

            return matrix;
        }

        /// <summary>
        /// 准备一个一行的矩阵
        /// </summary>
        /// <returns></returns>
        public static WfApprovalMatrix PrepareOneRowApprovalMatrixResourceDescriptor()
        {
            WfApprovalMatrix resource = new WfApprovalMatrix();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            resource.Rows.Add(PrepareOneRow(resource.PropertyDefinitions));

            return resource;
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

        private static SOARolePropertyRow PrepareOneRow(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRow row = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = string.Empty };

            row.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = "yangrui1" });
            row.Values.Add(new SOARolePropertyValue(pds["Approver2"]) { Value = "quym" });
            row.Values.Add(new SOARolePropertyValue(pds["Approver3"]) { Value = "liming" });

            return row;
        }

        private static SOARolePropertyRowCollection PrepareRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = "yangrui1" });
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
            row4.Values.Add(new SOARolePropertyValue(pds["Approver1"]) { Value = "yangrui1" });
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
