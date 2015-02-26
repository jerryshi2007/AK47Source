using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 节点描述
    /// </summary>
    public interface IWfMatrixActivity
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        string CodeName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }


        /// <summary>
        /// 表单地址
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 节点的类型
        /// </summary>
        WfMaxtrixActivityType ActivityType { get; set; }

        /// <summary>
        /// 更多属性设置
        /// </summary>
        IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// 条件设置列表
        /// </summary>
        IWfMatrixConditionGroupCollection Expression { get; set; }

        /// <summary>
        /// 节点相关的资源
        /// </summary>
        IWfMatrixCandidateCollection Candidates { get; set; }

       
    }
}
