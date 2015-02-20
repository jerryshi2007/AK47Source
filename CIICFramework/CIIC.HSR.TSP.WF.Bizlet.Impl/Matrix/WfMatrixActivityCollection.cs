using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixActivityCollection : List<IWfMatrixActivity>, IWfMatrixActivityCollection
    {
        /* 沈峥注释。排序在客户端完成，不需要
        /// <summary>
        /// 将指定的节点上移
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>新的序号</returns>
        public int MoveUp(string key)
        {
            var activity = this.Where(p => p.Key == key).FirstOrDefault();

            if (!CanMoveUp(key))
            {
                return activity.Sort;
            }

            var activitiesSorted = this.OrderBy(p => p.Sort).ToList();
            var preActivity = activitiesSorted[activitiesSorted.IndexOf(activity) - 1];

            int temp;
            temp = activity.Sort;
            activity.Sort = preActivity.Sort;
            preActivity.Sort = temp;

            return activity.Sort;
        }

        /// <summary>
        /// 将指定的节点下移动序号
        /// </summary>
        /// <param name="orginSort">节点Id</param>
        /// <returns>新的序号</returns>
        public int MoveDown(string key)
        {
            var activity = this.Where(p => p.Key == key).FirstOrDefault();

            if (!CanMoveDown(key))
            {
                return activity.Sort;
            }

            var activitiesSorted = this.OrderBy(p => p.Sort).ToList();
            var preActivity = activitiesSorted[activitiesSorted.IndexOf(activity) + 1];

            int temp;
            temp = activity.Sort;
            activity.Sort = preActivity.Sort;
            preActivity.Sort = temp;

            return activity.Sort;
        }

        /// <summary>
        /// 是否可以向上移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        public bool CanMoveUp(string key)
        {
            var activity = this.Where(p => p.Key == key).FirstOrDefault();
            var activitiesSorted = this.OrderBy(p => p.Sort).ToList();

            return activitiesSorted.IndexOf(activity) > 0;
        }

        /// <summary>
        /// 是否可以向下移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        public bool CanMoveDown(string key)
        {
            var activity = this.Where(p => p.Key == key).FirstOrDefault();
            var activitiesSorted = this.OrderBy(p => p.Sort).ToList();

            return activitiesSorted.IndexOf(activity) < this.Count - 1;
        }
        */

        /// <summary>
        /// 根据节点key删除一个节点。如果没有找到该节点，则忽略
        /// </summary>
        /// <param name="key">节点key</param>
        public void Remove(string key)
        {
            IWfMatrixActivity activity = this[key];

            if (key != null)
                this.Remove(activity);
        }

        /// <summary>
        /// 根据活动的Key的查找集合中的活动，如果没有找到，则返回null。
        /// Key的比较规则是大小写无关的
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IWfMatrixActivity this[string codeName]
        {
            get
            {
                return this.FirstOrDefault(p => string.Compare(p.CodeName, codeName, true) == 0);
            }
        }
    }
}
