using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 条件组
    /// </summary>
    public interface IWfMatrixConditionGroupCollection : IList<IWfMatrixConditionGroup>, IWfMatrixConditionConvertor
    {
        /// <summary>
        /// 逻辑关系
        /// </summary>
        LogicalRelation Relation { get; set; }

        /* 沈峥注释
        /// <summary>
        /// 组中的条件集合
        /// </summary>
        List<IWfMatrixConditionGroup> Conditions { get; }

        /// <summary>
        /// 获取指定的组
        /// </summary>
        /// <param name="id">组Id</param>
        /// <returns>指定的条件组</returns>
        IWfMatrixConditionGroup GetConditionCollectionById(Guid id);
        /// <summary>
        /// 移除一组条件
        /// </summary>
        /// <param name="id">组Id</param>
        void RemoveConditionCollectionById(Guid id);
        /// <summary>
        /// 快速添加一个表达式
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>新增的表达式组，组中只有condition一个表达式</returns>
        IWfMatrixConditionGroup AddConditon(IWfMatrixCondition condition);
        /// <summary>
        /// 解散一个组
        /// </summary>
        /// <param name="id">组Id</param>
        void DismissConditionCollection(Guid id);
         */
    }
}
