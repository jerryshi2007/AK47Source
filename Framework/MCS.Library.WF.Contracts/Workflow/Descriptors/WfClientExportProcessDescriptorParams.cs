using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    /// <summary>
    /// 导出流程定义参数的客户端对象
    /// </summary>
    public class WfClientExportProcessDescriptorParams
    {
        public WfClientExportProcessDescriptorParams()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixRoleAsPerson">是否将权限矩阵中的角色展开，变成人员</param>
        public WfClientExportProcessDescriptorParams(bool matrixRoleAsPerson)
        {
            this.MatrixRoleAsPerson = matrixRoleAsPerson;
        }

        /// <summary>
        /// 是否将权限矩阵中的角色展开，变成人员
        /// </summary>
        public bool MatrixRoleAsPerson
        {
            get;
            set;
        }
    }
}
