using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 动态流程
    /// </summary>
    public interface IWfMatrixProcess
    {
        /// <summary>
        /// 租户编码
        /// </summary>
        string TenantCode { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 流程key
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 大分类
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// 小分类
        /// </summary>
        string ProgramName { get; set; }

        /// <summary>
        /// 表单地址
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 更多属性设置
        /// </summary>
        IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// 额外的参数定义
        /// </summary>
        IWfMatrixParameterDefinitionCollection ParameterDefinitions { get; set; }

        /// <summary>
        /// 系统参数
        /// </summary>
        IWfMatrixParameterDefinitionCollection GlobalParameterDefinitions { get; }

        /// <summary>
        /// 节点列表
        /// </summary>
        IWfMatrixActivityCollection Activities { get; set; }

        /// <summary>
        /// 初始化系统参数
        /// </summary>
        void InitGlobalParameter();

      

    }
}
