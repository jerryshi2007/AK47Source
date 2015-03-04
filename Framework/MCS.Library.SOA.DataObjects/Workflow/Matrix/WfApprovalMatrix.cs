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
    }
}
