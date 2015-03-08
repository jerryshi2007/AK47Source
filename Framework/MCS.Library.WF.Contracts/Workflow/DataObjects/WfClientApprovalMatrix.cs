using MCS.Library.Core;
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
    /// 客户端审批矩阵
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientApprovalMatrix : IWfClientMatrixContainer
    {
        private WfClientRolePropertyDefinitionCollection _PropertyDefinitions = null;
        private WfClientRolePropertyRowCollection _Rows = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfClientApprovalMatrix()
        {
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        public WfClientApprovalMatrix(DataTable table)
        {
            this.FromDataTable(table);
        }

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
        public WfClientRolePropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                if (this._PropertyDefinitions == null)
                    this._PropertyDefinitions = new WfClientRolePropertyDefinitionCollection();

                return this._PropertyDefinitions;
            }
        }

        /// <summary>
        /// 矩阵的行
        /// </summary>
        public WfClientRolePropertyRowCollection Rows
        {
            get
            {
                if (this._Rows == null)
                    this._Rows = new WfClientRolePropertyRowCollection();

                return this._Rows;
            }
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        public void FromDataTable(DataTable table)
        {
            table.NullCheck("table");

            this.PropertyDefinitions.FromDataColumns(table.Columns);
            this.Rows.FromDataTable(table.Rows, this.PropertyDefinitions);
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        public WfClientMatrixType MatrixType
        {
            get
            {
                return this.PropertyDefinitions.MatrixType;
            }
        }
    }
}
