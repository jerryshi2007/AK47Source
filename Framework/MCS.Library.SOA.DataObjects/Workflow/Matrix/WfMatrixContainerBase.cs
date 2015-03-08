using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 矩阵容器的虚基类
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public abstract class WfMatrixContainerBase : IWfMatrixContainer
    {
        private SOARolePropertyDefinitionCollection _PropertyDefinitions = null;
        private SOARolePropertyRowCollection _Rows = null;

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
            internal protected set
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
            internal protected set
            {
                this._Rows = value;
            }
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        public WfMatrixType MatrixType
        {
            get
            {
                return this.PropertyDefinitions.MatrixType;
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
    }
}
