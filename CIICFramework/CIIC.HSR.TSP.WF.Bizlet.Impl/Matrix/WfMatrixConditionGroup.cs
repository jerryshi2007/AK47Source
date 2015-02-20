using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixConditionGroup : List<IWfMatrixCondition>, IWfMatrixConditionGroup
    {
        private LogicalRelation _Relation = LogicalRelation.And;

        /// <summary>
        /// 逻辑关系
        /// </summary>
        public LogicalRelation Relation
        {
            get
            {
                return this._Relation;
            }
            set
            {
                this._Relation = value;
            }
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <returns>字符串表达式</returns>
        public string ToExpression()
        {
            StringBuilder strB = new StringBuilder();
            string logicalSign = GetLogicalRelation(this._Relation);

            foreach (IWfMatrixCondition condition in this)
            {
                if (strB.Length > 0)
                    strB.AppendFormat(" {0} ", logicalSign);

                strB.AppendFormat("({0})", condition.ToExpression());
            }

            return strB.ToString();
        }

        /// <summary>
        /// 获取关系运算符
        /// </summary>
        /// <param name="relation">逻辑关系</param>
        /// <returns></returns>
        public static string GetLogicalRelation(LogicalRelation relation)
        {
            string sign = string.Empty;
            switch (relation)
            {
                case LogicalRelation.And:
                    sign = "&&";
                    break;
                case LogicalRelation.Or:
                    sign = "||";
                    break;
            }

            return sign;
        }

        /* 沈峥注释
        /// <summary>
        /// 表达式集合Id
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 序号
        /// </summary>
        public new int Sort
        {
            get;
            set;
        }

        /// <summary>
        /// 将指定的条件上移
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>新的序号</returns>
        public int MoveUp(Guid id)
        {
            var condition = this.Where(p => p.Id == id).FirstOrDefault();

            if (!CanMoveUp(id))
            {
                return condition.Sort;
            }

            var conditionsSorted = this.OrderBy(p => p.Sort).ToList();
            var precondition = conditionsSorted[conditionsSorted.IndexOf(condition) - 1];

            int temp;
            temp = condition.Sort;
            condition.Sort = precondition.Sort;
            precondition.Sort = temp;

            return condition.Sort;
        }

        /// <summary>
        /// 将指定的条件下移动序号
        /// </summary>
        /// <param name="orginSort">节点Id</param>
        /// <returns>新的序号</returns>
        public int MoveDown(Guid id)
        {
            var condition = this.Where(p => p.Id == id).FirstOrDefault();

            if (!CanMoveDown(id))
            {
                return condition.Sort;
            }

            var conditionsSorted = this.OrderBy(p => p.Sort).ToList();
            var precondition = conditionsSorted[conditionsSorted.IndexOf(condition) + 1];

            int temp;
            temp = condition.Sort;
            condition.Sort = precondition.Sort;
            precondition.Sort = temp;

            return condition.Sort;
        }

        /// <summary>
        /// 是否可以向上移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        public bool CanMoveUp(Guid id)
        {
            var condition = this.Where(p => p.Id == id).FirstOrDefault();
            var conditionsSorted = this.OrderBy(p => p.Sort).ToList();

            return conditionsSorted.IndexOf(condition) > 0;
        }

        /// <summary>
        /// 是否可以向下移动
        /// </summary>
        /// <param name="id">节点Id</param>
        /// <returns>可以移动，返回true；否则，返回false</returns>
        public bool CanMoveDown(Guid id)
        {
            var condition = this.Where(p => p.Id == id).FirstOrDefault();
            var conditionsSorted = this.OrderBy(p => p.Sort).ToList();

            return conditionsSorted.IndexOf(condition) < this.Count - 1;
        }

        /// <summary>
        /// 根据条件Id删除一个节点
        /// </summary>
        /// <param name="id">节点Id</param>
        public void RemoveCondition(Guid id)
        {
            var condition = this.Where(p => p.Id == id).FirstOrDefault();
            this.Remove(condition);
        }

        /// <summary>
        /// 根据Id获取参数定义
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>参数定义信息</returns>
        public IWfMatrixCondition GetById(Guid id)
        {
            return this.Where(p => p.Id == id).FirstOrDefault();
        }
        */
    }
}
