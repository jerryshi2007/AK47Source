using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 审批矩阵
    /// </summary>
    [Serializable]
    public class WfApprovalMatrix : WfMatrixContainerBase
    {
        public WfApprovalMatrix()
        {
        }

        public WfApprovalMatrix(DataTable table)
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
    }
}
