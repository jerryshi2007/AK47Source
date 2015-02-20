using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 流程相关的变量定义
    /// </summary>
    public interface IWfMatrixParameterDefinition
    {
        /* 沈峥注释
        /// <summary>
        /// 唯一标识
        /// </summary>
        Guid Id { get; set; }
        */

        /// <summary>
        /// 内部名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        object DefaultValue { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        ParaType ParameterType { get; set; }
    }
}
