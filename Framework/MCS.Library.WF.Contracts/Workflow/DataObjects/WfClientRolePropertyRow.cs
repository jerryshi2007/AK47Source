using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    /// <summary>
    /// 角色中包含的人，目前只支持人员
    /// </summary>
    public enum WfClientRoleOperatorType
    {
        User = 2,

        //Group = 4
        /// <summary>
        /// 角色和动态角色
        /// </summary>
        Role = 8,

        /// <summary>
        /// 管理单元角色
        /// </summary>
        AURole = 16,
    }

    [DataContract]
    [Serializable]
    public class WfClientRolePropertyRow : TableRowBase<WfClientRolePropertyDefinition, WfClientRolePropertyValue, WfClientRolePropertyValueCollection, string>
    {
        public static readonly char[] OperatorSplitters = new char[] { ',', '，' };

        private WfClientRoleOperatorType _OperatorType = WfClientRoleOperatorType.User;

        public int RowNumber
        {
            get;
            set;
        }

        public string Operator
        {
            get;
            set;
        }

        public WfClientRoleOperatorType OperatorType
        {
            get
            {
                return this._OperatorType;
            }
            set
            {
                if (value == 0)
                {
                    this._OperatorType = WfClientRoleOperatorType.User;
                }
                else
                {
                    this._OperatorType = value;
                }
            }
        }

        /// <summary>
        /// 是否可以合并(判断是否有IsMergeable属性)
        /// </summary>
        /// <returns></returns>
        public bool IsMergeable()
        {
            bool result = false;

            string mergeable = this.Values.GetValue("IsMergeable", "False");

            try
            {
                result = (bool)DataConverter.ChangeType(mergeable, typeof(bool));
            }
            catch (System.Exception)
            {
            }

            return result;
        }
    }

    [Serializable]
    public class WfClientRolePropertyRowCollection : TableRowCollectionBase<WfClientRolePropertyRow, WfClientRolePropertyDefinition, WfClientRolePropertyValue, WfClientRolePropertyValueCollection, string>
    {
        public void FillColumnInfoToRowValues(WfClientRolePropertyDefinitionCollection columns)
        {
            foreach (WfClientRolePropertyRow row in this)
            {
                foreach (WfClientRolePropertyValue pv in row.Values)
                {
                    WfClientRolePropertyDefinition column = columns[pv.Column.Name];

                    pv.SetColumnInfo(column);
                }
            }
        }

        /// <summary>
        /// 从DataRow构造行信息
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="definitions"></param>
        internal void FromDataTable(DataRowCollection rows, IEnumerable<WfClientRolePropertyDefinition> definitions)
        {
            rows.NullCheck("rows");
            definitions.NullCheck("definitions");

            this.Clear();

            int rowIndex = 1;

            foreach (DataRow row in rows)
            {
                WfClientRolePropertyRow mRow = new WfClientRolePropertyRow() { RowNumber = rowIndex++ };

                foreach (WfClientRolePropertyDefinition dimension in definitions)
                {
                    WfClientRolePropertyValue mCell = new WfClientRolePropertyValue(dimension);
                    mCell.Value = row[dimension.Name].ToString();

                    switch (dimension.Name)
                    {
                        case "Operator":
                            mRow.Operator = row[dimension.Name].ToString();
                            break;
                        case "OperatorType":
                            WfClientRoleOperatorType opType = WfClientRoleOperatorType.User;
                            Enum.TryParse(row[dimension.Name].ToString(), out opType);
                            mRow.OperatorType = opType;
                            break;
                        default:
                            break;
                    }

                    mRow.Values.Add(mCell);
                }

                this.Add(mRow);
            }
        }
    }
}
