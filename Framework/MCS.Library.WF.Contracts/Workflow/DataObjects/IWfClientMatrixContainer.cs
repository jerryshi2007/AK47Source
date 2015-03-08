using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    /// <summary>
    /// 矩阵的类型
    /// </summary>
    public enum WfClientMatrixType
    {
        /// <summary>
        /// 角色矩阵
        /// </summary>
        RoleMatrix = 0,

        /// <summary>
        /// 活动矩阵
        /// </summary>
        ActivityMatrix = 1,

        /// <summary>
        /// 审批矩阵
        /// </summary>
        ApprovalMatrix = 2,
    }

    /// <summary>
    /// 客户端矩阵容器所需要实现的接口
    /// </summary>
    public interface IWfClientMatrixContainer
    {
        /// <summary>
        /// 属性的定义（列）
        /// </summary>
        WfClientRolePropertyDefinitionCollection PropertyDefinitions
        {
            get;
        }

        /// <summary>
        /// 行信息
        /// </summary>
        WfClientRolePropertyRowCollection Rows
        {
            get;
        }

        /// <summary>
        /// 从DataTable构造
        /// </summary>
        /// <param name="table"></param>
        void FromDataTable(DataTable table);

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        WfClientMatrixType MatrixType
        {
            get;
        }
    }
}
