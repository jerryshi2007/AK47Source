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
            int maxRowNumber = GetMaxRowNumber(amRows);

            List<SOARolePropertyRow> newAmRows = new List<SOARolePropertyRow>();

            foreach (SOARolePropertyRow apRow in apRows)
            {
                int columnIndex = 1;

                while (columnIndex < apDefinitions.Count)
                {
                    SOARolePropertyRow templateRow = null;

                    SOARolePropertyRow amRow = FindMatchedActivityMatrixRow(amRows, apDefinitions[columnIndex].Name, apRow, out templateRow);

                    string apUser = apRow.Values.GetValue(apDefinitions[columnIndex].Name, string.Empty);

                    if (amRow == null && apUser.IsNotEmpty())
                    {
                        maxActivitySN += 10;

                        amRow = CreateNewActivityMatrixRow(maxActivitySN, ++maxRowNumber, amDefinitions, templateRow);

                        newAmRows.Add(amRow);
                    }

                    if (amRow != null)
                        MergeToActivityMatrixRow(amRow, amDefinitions, apUser);

                    columnIndex++;
                }
            }

            amRows.CopyFrom(newAmRows);
        }

        private static SOARolePropertyRow CreateNewActivityMatrixRow(int activitySN, int rowNumber, SOARolePropertyDefinitionCollection amDefinitions, SOARolePropertyRow templateRow)
        {
            SOARolePropertyRow amRow = null;

            if (templateRow != null)
                amRow = new SOARolePropertyRow(templateRow, rowNumber);
            else
                amRow = new SOARolePropertyRow() { RowNumber = rowNumber };

            amRow.OperatorType = SOARoleOperatorType.User;
            amRow.Operator = string.Empty;

            SetCellValue(amRow, amDefinitions, "Operator", string.Empty);
            SetCellValue(amRow, amDefinitions, "ActivitySN", activitySN.ToString());

            return amRow;
        }

        private static SOARolePropertyRow FindMatchedActivityMatrixRow(
            IEnumerable<SOARolePropertyRow> amRows,
            string apColumnName,
            SOARolePropertyRow apRow,
            out SOARolePropertyRow templateRow)
        {
            SOARolePropertyRow result = null;
            templateRow = null;

            foreach (SOARolePropertyRow amRow in amRows)
            {
                string activityCode = amRow.Values.GetValue("ActivityCode", string.Empty);

                if (activityCode.IsNotEmpty())
                {
                    if (string.Compare(apColumnName, activityCode, true) == 0)
                    {
                        if (amRow.OperatorType == SOARoleOperatorType.User)
                        {
                            result = amRow;
                            break;
                        }
                        else
                        {
                            templateRow = amRow;
                        }
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

        private static int GetMaxRowNumber(IEnumerable<SOARolePropertyRow> amRows)
        {
            int result = 0;

            foreach (SOARolePropertyRow row in amRows)
            {
                if (row.RowNumber > result)
                    result = row.RowNumber;
            }

            return result;
        }
    }
}
