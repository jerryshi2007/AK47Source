using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程设计器操作类型定义
    /// </summary>
    public enum WfDesignerOperationType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumItemDescription("未指定")]
        None,

        /// <summary>
        /// 创建流程模板
        /// </summary>
        [EnumItemDescription("创建流程模板")]
        CreateTemplate,

        /// <summary>
        /// 修改流程模板
        /// </summary>
        [EnumItemDescription("修改流程模板")]
        ModifyTemplate,

        /// <summary>
        /// 删除流程模板
        /// </summary>
        [EnumItemDescription("删除流程模板")]
        DeleteTemplate,

        /// <summary>
        /// 导入流程模板
        /// </summary>
        [EnumItemDescription("导入流程模板")]
        ImportTemplate,

        /// <summary>
        /// 导入审批矩阵
        /// </summary>
        [EnumItemDescription("导入审批矩阵")]
        ImportApprovalMatrix,

        /// <summary>
        /// 删除审批矩阵
        /// </summary>
        [EnumItemDescription("删除审批矩阵")]
        DeleteApprovalMatrix,

        /// <summary>
        /// 清除所有流程
        /// </summary>
        [EnumItemDescription("清除所有流程")]
        ClearAll
    }
}
