using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 矩阵合并相关的
    /// </summary>
    public static class WfMatrixExtensions
    {
        /// <summary>
        /// 将活动矩阵与审批矩阵进行合并，以第一个矩阵的列定义为准
        /// </summary>
        /// <param name="activieyMatrixResource"></param>
        /// <param name="container"></param>
        public static void MergeActivityMatrix(this IWfMatrixContainer activieyMatrixResource, IWfMatrixContainer container)
        {
            activieyMatrixResource.NullCheck("activieyMatrixResource");
            container.NullCheck("container");

            activieyMatrixResource.Rows.MergeActivityMatrix(
                    activieyMatrixResource.PropertyDefinitions,
                    container.Rows,
                    container.PropertyDefinitions
                );
        }

        /// <summary>
        /// 将活动矩阵与审批矩阵进行合并，以第一个矩阵的列定义为准
        /// </summary>
        /// <param name="amRows"></param>
        /// <param name="amDefinitions"></param>
        /// <param name="apRows"></param>
        /// <param name="apDefinitions"></param>
        public static void MergeActivityMatrix(this SOARolePropertyRowCollection amRows, SOARolePropertyDefinitionCollection amDefinitions, IEnumerable<SOARolePropertyRow> apRows, SOARolePropertyDefinitionCollection apDefinitions)
        {
            amDefinitions.NullCheck("amDefinitions");
            amRows.NullCheck("amRows");
            apDefinitions.NullCheck("apDefinitions");
            apRows.NullCheck("apRows");

            int maxRowNumber = GetMaxRowNumber(amRows);

            foreach (SOARolePropertyRow apRow in apRows)
            {
                SOARolePropertyRow newRow = new SOARolePropertyRow(amRows.Role);

                newRow.RowNumber = ++maxRowNumber;
                newRow.OperatorType = apRow.OperatorType;
                newRow.Operator = apRow.Operator;

                foreach (SOARolePropertyValue srv in apRow.Values)
                {
                    if (amDefinitions.ContainsKey(srv.Column.Name))
                    {
                        SOARolePropertyValue newValue = new SOARolePropertyValue(amDefinitions[srv.Column.Name]);

                        newValue.Value = srv.Value;

                        newRow.Values.Add(newValue);
                    }
                }
            }
        }

        /// <summary>
        /// 将活动矩阵与审批矩阵进行合并
        /// </summary>
        /// <param name="activieyMatrixResource"></param>
        /// <param name="approvalMatrix"></param>
        public static void MergeApprovalMatrix(this IWfMatrixContainer activieyMatrixResource, IWfMatrixContainer approvalMatrix)
        {
            activieyMatrixResource.NullCheck("activieyMatrixResource");
            approvalMatrix.NullCheck("approvalMatrix");

            activieyMatrixResource.Rows.MergeApprovalMatrix(
                    activieyMatrixResource.PropertyDefinitions,
                    approvalMatrix.Rows,
                    approvalMatrix.PropertyDefinitions
                );
        }

        /// <summary>
        /// 将活动矩阵与审批矩阵进行合并
        /// </summary>
        /// <param name="amRows"></param>
        /// <param name="amDefinitions"></param>
        /// <param name="apRows"></param>
        /// <param name="apDefinitions"></param>
        public static void MergeApprovalMatrix(this SOARolePropertyRowCollection amRows, SOARolePropertyDefinitionCollection amDefinitions, IEnumerable<SOARolePropertyRow> apRows, SOARolePropertyDefinitionCollection apDefinitions)
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
                    List<SOARolePropertyRow> amUserRows = FindMatchedActivityMatrixUserRows(amRows, apDefinitions[columnIndex].Name, apRow);
                    List<SOARolePropertyRow> amTemplateRows = FindMatchedActivityMatrixTemplateRows(amRows, apDefinitions[columnIndex].Name, apRow);

                    string apUser = apRow.Values.GetValue(apDefinitions[columnIndex].Name, string.Empty);

                    if (amUserRows.Count == 0 && apUser.IsNotEmpty())
                    {
                        SOARolePropertyRow templateRow = amTemplateRows.LastOrDefault();

                        maxActivitySN += 10;
                        SOARolePropertyRow amRow = CreateNewActivityMatrixRow(maxActivitySN, ++maxRowNumber, amDefinitions, templateRow);

                        newAmRows.Add(amRow);

                        MergeToActivityMatrixRow(amRow, amDefinitions, apUser);
                    }
                    else
                    {
                        foreach (SOARolePropertyRow amRow in amUserRows)
                            MergeToActivityMatrixRow(amRow, amDefinitions, apUser);
                    }

                    columnIndex++;
                }
            }

            amRows.CopyFrom(newAmRows);
        }

        ///// <summary>
        ///// 将活动矩阵与审批矩阵进行合并
        ///// </summary>
        ///// <param name="amRows"></param>
        ///// <param name="amDefinitions"></param>
        ///// <param name="apRows"></param>
        ///// <param name="apDefinitions"></param>
        //public static void MergeApprovalMatrix(this SOARolePropertyRowCollection amRows, SOARolePropertyDefinitionCollection amDefinitions, IEnumerable<SOARolePropertyRow> apRows, SOARolePropertyDefinitionCollection apDefinitions)
        //{
        //    amDefinitions.NullCheck("amDefinitions");
        //    amRows.NullCheck("amRows");
        //    apDefinitions.NullCheck("apDefinitions");
        //    apRows.NullCheck("apRows");

        //    int maxActivitySN = GetMaxActivitySN(amRows);
        //    int maxRowNumber = GetMaxRowNumber(amRows);

        //    List<SOARolePropertyRow> newAmRows = new List<SOARolePropertyRow>();

        //    foreach (SOARolePropertyRow apRow in apRows)
        //    {
        //        int columnIndex = 1;

        //        while (columnIndex < apDefinitions.Count)
        //        {
        //            SOARolePropertyRow templateRow = null;

        //            SOARolePropertyRow amRow = FindMatchedActivityMatrixRow(amRows, apDefinitions[columnIndex].Name, apRow, out templateRow);

        //            string apUser = apRow.Values.GetValue(apDefinitions[columnIndex].Name, string.Empty);

        //            if (amRow == null && apUser.IsNotEmpty())
        //            {
        //                maxActivitySN += 10;

        //                amRow = CreateNewActivityMatrixRow(maxActivitySN, ++maxRowNumber, amDefinitions, templateRow);

        //                newAmRows.Add(amRow);
        //            }

        //            if (amRow != null)
        //                MergeToActivityMatrixRow(amRow, amDefinitions, apUser);

        //            columnIndex++;
        //        }
        //    }

        //    amRows.CopyFrom(newAmRows);
        //}

        private static SOARolePropertyRow CreateNewActivityMatrixRow(int activitySN, int rowNumber, SOARolePropertyDefinitionCollection amDefinitions, SOARolePropertyRow templateRow)
        {
            SOARolePropertyRow amRow = null;

            if (templateRow != null)
                amRow = new SOARolePropertyRow(templateRow, rowNumber);
            else
                amRow = new SOARolePropertyRow() { RowNumber = rowNumber };

            amRow.OperatorType = SOARoleOperatorType.User;
            amRow.Operator = string.Empty;

            SetCellValue(amRow, amDefinitions, SOARolePropertyDefinition.OperatorColumn, string.Empty);
            SetCellValue(amRow, amDefinitions, SOARolePropertyDefinition.ActivitySNColumn, activitySN.ToString());

            return amRow;
        }

        //private static SOARolePropertyRow FindMatchedActivityMatrixRow(
        //    IEnumerable<SOARolePropertyRow> amRows,
        //    string apColumnName,
        //    SOARolePropertyRow apRow,
        //    out SOARolePropertyRow templateRow)
        //{
        //    SOARolePropertyRow result = null;
        //    templateRow = null;

        //    foreach (SOARolePropertyRow amRow in amRows)
        //    {
        //        string activityCode = amRow.Values.GetValue(SOARolePropertyDefinition.ActivityCodeColumn, string.Empty);

        //        if (activityCode.IsNotEmpty())
        //        {
        //            if (string.Compare(apColumnName, activityCode, true) == 0)
        //            {
        //                if (amRow.OperatorType == SOARoleOperatorType.User)
        //                {
        //                    result = amRow;
        //                    break;
        //                }
        //                else
        //                {
        //                    templateRow = amRow;
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}

        private static List<SOARolePropertyRow> FindMatchedActivityMatrixUserRows(
            IEnumerable<SOARolePropertyRow> amRows,
            string apColumnName,
            SOARolePropertyRow apRow)
        {
            return FindMatchedActivityMatrixRows(amRows, apColumnName, apRow, (amRow) => amRow.OperatorType == SOARoleOperatorType.User);
        }

        private static List<SOARolePropertyRow> FindMatchedActivityMatrixTemplateRows(
            IEnumerable<SOARolePropertyRow> amRows,
            string apColumnName,
            SOARolePropertyRow apRow)
        {
            return FindMatchedActivityMatrixRows(amRows, apColumnName, apRow, (amRow) => amRow.OperatorType != SOARoleOperatorType.User);
        }

        private static List<SOARolePropertyRow> FindMatchedActivityMatrixRows(
            IEnumerable<SOARolePropertyRow> amRows,
            string apColumnName,
            SOARolePropertyRow apRow,
            Func<SOARolePropertyRow, bool> condition)
        {
            List<SOARolePropertyRow> result = new List<SOARolePropertyRow>();

            foreach (SOARolePropertyRow amRow in amRows)
            {
                string activityCode = amRow.Values.GetValue(SOARolePropertyDefinition.ActivityCodeColumn, string.Empty);

                if (activityCode.IsNotEmpty())
                {
                    if (string.Compare(apColumnName, activityCode, true) == 0)
                    {
                        if (condition(amRow))
                            result.Add(amRow);
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
            SetCellValue(amRow, amDefinitions, SOARolePropertyDefinition.OperatorTypeColumn, SOARoleOperatorType.User.ToString());

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
            string columnName = SOARolePropertyDefinition.OperatorColumn;

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
                int activitySN = row.Values.GetValue<int>(SOARolePropertyDefinition.ActivitySNColumn, 0);

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
