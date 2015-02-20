using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    public interface IWfMatrixActivityCollection : IList<IWfMatrixActivity>
    {
        /// <summary>
        /// 根据活动的Key的查找集合中的活动，如果没有找到，则返回null。
        /// Key的比较规则是大小写无关的
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IWfMatrixActivity this[string key]
        {
            get;
        }

        /// <summary>
        /// 根据节点key删除一个节点。如果没有找到该节点，则忽略
        /// </summary>
        /// <param name="key">节点key</param>
        void Remove(string key);

        /* 沈峥注释。排序在客户端完成，不需要
        /// <summary>
        /// 将指定的节点上移
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>新的序号</returns>
        int MoveUp(string key);
        
        /// <summary>
        /// 将指定的节点下移动序号
        /// </summary>
        /// <param name="key">节点Id</param>
        /// <returns>新的序号</returns>
        int MoveDown(string key);
        
        /// <summary>
        /// 是否可以向上移动
        /// </summary>
        /// <param name="key">节点key</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        bool CanMoveUp(string key);
        
        /// <summary>
        /// 是否可以向下移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        bool CanMoveDown(string key);
        */
    }
}
