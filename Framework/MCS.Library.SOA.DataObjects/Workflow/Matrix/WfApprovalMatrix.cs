using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 审批矩阵
    /// </summary>
    [Serializable]
    public class WfApprovalMatrix
    {
        private SOARolePropertyDefinitionCollection _PropertyDefinitions = null;
        private SOARolePropertyRowCollection _Rows = null;

        /// <summary>
        /// 矩阵的ID
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 矩阵的列定义
        /// </summary>
        public SOARolePropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                if (this._PropertyDefinitions == null)
                    this._PropertyDefinitions = new SOARolePropertyDefinitionCollection();

                return this._PropertyDefinitions;
            }
            internal set
            {
                this._PropertyDefinitions = value;
            }
        }

        /// <summary>
        /// 矩阵的行
        /// </summary>
        public SOARolePropertyRowCollection Rows
        {
            get
            {
                if (this._Rows == null)
                    this._Rows = new SOARolePropertyRowCollection();

                return this._Rows;
            }
            internal set
            {
                this._Rows = value;
            }
        }

        private static void MergeToActivityMatrix(SOARolePropertyDefinitionCollection amDefinitions, SOARolePropertyRowCollection amRows, SOARolePropertyDefinitionCollection apDefinitions, IEnumerable<SOARolePropertyRow> apRows)
        {
            foreach (SOARolePropertyRow apRow in apRows)
            {
                SOARolePropertyRow amRow = FindMatchedActivityMatrixRow(amDefinitions, amRows, apDefinitions, apRow);

                if (amRow != null)
                {
                    MergeToActivityMatrixRow(amDefinitions, amRow, apRow);
                }
                else
                {
                    amRow = new SOARolePropertyRow() { RowNumber = amDefinitions.Count, OperatorType = SOARoleOperatorType.User };
                    amDefinitions.Add(amRow);
                }

                MergeToActivityMatrixRow(amDefinitions, amRow, apRow);
            }
        }

        private static SOARolePropertyRow FindMatchedActivityMatrixRow(
            SOARolePropertyDefinitionCollection amDefinitions,
            IEnumerable<SOARolePropertyRow> amRows,
            SOARolePropertyDefinitionCollection apDefinitions,
            SOARolePropertyRow apRow)
        {
            SOARolePropertyRow result = null;

            if (apDefinitions.Count > 1)
            {
                foreach (SOARolePropertyRow amRow in amRows)
                {
                    //for (int i = 1; i < apDefinitions.Count; i++)
                    //{
                    //    string activieyCode = amRows.Values.GetValue(apDefinitions[i].Name, string.Empty);
                    //}
                }
            }

            return result;
        }

        private static void MergeToActivityMatrixRow(SOARolePropertyDefinitionCollection amDefinitions, SOARolePropertyRow amRow, SOARolePropertyRow apRow)
        {
            if (amDefinitions.ContainsKey("OperatorType"))
                amRow.Values.Add(new SOARolePropertyValue(amDefinitions["OperatypeType"]) { Value = SOARoleOperatorType.User.ToString() });

            if (amDefinitions.ContainsKey("Operator"))
                amRow.Values.Add(new SOARolePropertyValue(amDefinitions["Operator"]) { Value = apRow.Operator });
        }
    }
}
