using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    public interface IWfMatrixConditionGroup : IList<IWfMatrixCondition>, IWfMatrixConditionConvertor
    {
        /// <summary>
        /// 逻辑关系
        /// </summary>
        LogicalRelation Relation { get; set; }

        /* 沈峥注释
        /// <summary>
        /// 表达式集合Id
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        int Sort { get; set; }

        /// <summary>
        /// 根据Id获取条件信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>条件信息</returns>
        IWfMatrixCondition GetById(Guid id);

        /// <summary>
        /// 将指定的节点上移
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>新的序号</returns>
        int MoveUp(Guid id);

        /// <summary>
        /// 将指定的节点下移动序号
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>新的序号</returns>
        int MoveDown(Guid id);

        /// <summary>
        /// 是否可以向上移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        bool CanMoveUp(Guid id);

        /// <summary>
        /// 是否可以向下移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        bool CanMoveDown(Guid id);

        /// <summary>
        /// 根据节点Id删除一个节点
        /// </summary>
        /// <param name="id">节点Id</param>
        void RemoveCondition(Guid id);
        */
    }
}
