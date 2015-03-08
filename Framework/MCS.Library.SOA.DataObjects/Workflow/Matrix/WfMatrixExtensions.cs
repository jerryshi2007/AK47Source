using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public static class WfMatrixExtensions
    {
        public static void MergeApprovalMatrix(this IWfMatrixContainer activieyMatrixResource, IWfMatrixContainer approvalMatrix)
        {
            activieyMatrixResource.NullCheck("activieyMatrixResource");
            approvalMatrix.NullCheck("approvalMatrix");

            activieyMatrixResource.Rows.MergeToActivityMatrix(
                    activieyMatrixResource.PropertyDefinitions,
                    approvalMatrix.Rows,
                    approvalMatrix.PropertyDefinitions
                );
        }

        public static void MergeToActivityMatrix(this SOARolePropertyRowCollection amRows, SOARolePropertyDefinitionCollection amDefinitions, IEnumerable<SOARolePropertyRow> apRows, SOARolePropertyDefinitionCollection apDefinitions)
        {
            amDefinitions.NullCheck("amDefinitions");
            amRows.NullCheck("amRows");
            apDefinitions.NullCheck("apDefinitions");
            apRows.NullCheck("apRows");

            int maxActivitySN = GetMaxActivitySN(amRows);

            List<SOARolePropertyRow> newAmRows = new List<SOARolePropertyRow>();

            foreach (SOARolePropertyRow apRow in apRows)
            {
                int columnIndex = 1;

                while (columnIndex < apDefinitions.Count)
                {
                    SOARolePropertyRow amRow = FindMatchedActivityMatrixRow(amRows, apDefinitions[columnIndex].Name, apRow);

                    string apUser = apRow.Values.GetValue(apDefinitions[columnIndex].Name, string.Empty);

                    if (amRow == null && apUser.IsNotEmpty())
                    {
                        amRow = new SOARolePropertyRow() { RowNumber = amDefinitions.Count, OperatorType = SOARoleOperatorType.User };

                        maxActivitySN += 10;

                        SetCellValue(amRow, amDefinitions, "ActivitySN", maxActivitySN.ToString());

                        newAmRows.Add(amRow);
                    }

                    if (amRow != null)
                        MergeToActivityMatrixRow(amRow, amDefinitions, apUser);

                    columnIndex++;
                }
            }

            amRows.CopyFrom(newAmRows);
        }

        private static SOARolePropertyRow FindMatchedActivityMatrixRow(
            IEnumerable<SOARolePropertyRow> amRows,
            string apColumnName,
            SOARolePropertyRow apRow)
        {
            SOARolePropertyRow result = null;

            foreach (SOARolePropertyRow amRow in amRows)
            {
                string activityCode = amRow.Values.GetValue("ActivityCode", string.Empty);

                if (activityCode.IsNotEmpty())
                {
                    if (string.Compare(apColumnName, activityCode, true) == 0)
                    {
                        result = amRow;
                        break;
                    }
                }
            }

            return result;
        }

        private static void MergeToActivityMatrixRow(
            SOARolePropertyRow amRow,
            SOARolePropertyDefinitionCollection amDefinitions,
            string apUser)
        {
            SetCellValue(amRow, amDefinitions, "OperatorType", SOARoleOperatorType.User.ToString());

            AppendOperator(amRow, amDefinitions, apUser);
        }

        private static void SetCellValue(SOARolePropertyRow row, SOARolePropertyDefinitionCollection columns, string columnName, string cellValue)
        {
            if (columns.ContainsKey(columnName))
            {
                SOARolePropertyValue pValue = row.Values.FindByColumnName(columnName);

                if (pValue == null)
                {
                    pValue = new SOARolePropertyValue(columns[columnName]);
                    row.Values.Add(pValue);
                }

                pValue.Value = cellValue;
            }
        }

        private static void AppendOperator(SOARolePropertyRow row, SOARolePropertyDefinitionCollection columns, string apUser)
        {
            string columnName = "Operator";

            if (columns.ContainsKey(columnName))
            {
                SOARolePropertyValue pValue = row.Values.FindByColumnName(columnName);

                if (pValue == null)
                {
                    pValue = new SOARolePropertyValue(columns[columnName]);
                    row.Values.Add(pValue);
                }

                if (apUser.IsNotEmpty())
                {
                    pValue.Value = pValue.Value.NullOrEmptyIs(row.Operator);

                    if (pValue.Value.IsNotEmpty())
                        pValue.Value += ",";
                    else
                        pValue.Value = string.Empty;

                    pValue.Value += apUser;

                    row.Operator = pValue.Value;
                }
            }
        }

        private static int GetMaxActivitySN(IEnumerable<SOARolePropertyRow> amRows)
        {
            int result = 0;

            foreach (SOARolePropertyRow row in amRows)
            {
                int activitySN = row.Values.GetValue<int>("ActivitySN", 0);

                if (activitySN > result)
                    result = activitySN;
            }

            return result;
        }
    }
}
