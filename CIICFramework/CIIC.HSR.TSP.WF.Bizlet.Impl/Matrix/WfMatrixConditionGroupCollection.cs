using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixConditionGroupCollection : List<IWfMatrixConditionGroup>, IWfMatrixConditionGroupCollection
    {
        private LogicalRelation _Relation = LogicalRelation.Or;

        /// <summary>
        /// 逻辑关系
        /// </summary>
        public LogicalRelation Relation
        {
            get
            {
                return _Relation;
            }
            set
            {
                _Relation = value;
            }
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <returns>字符串表达式</returns>
        public string ToExpression()
        {
            StringBuilder strB = new StringBuilder();
            string logicalSign = WfMatrixConditionGroup.GetLogicalRelation(this._Relation);

            foreach (IWfMatrixConditionGroup condition in this)
            {
                if (strB.Length > 0)
                    strB.AppendFormat(" {0} ", logicalSign);

                strB.AppendFormat("({0})", condition.ToExpression());
            }

            return strB.ToString();
        }

        /* 沈峥注释
        /// <summary>
        /// 组中的条件集合
        /// </summary>
        public List<IWfMatrixConditionGroup> Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                _Conditions = value;
            }
        }

        /// <summary>
        /// 获取指定的组
        /// </summary>
        /// <param name="id">组Id</param>
        /// <returns>指定的条件组</returns>
        public IWfMatrixConditionGroup GetConditionCollectionById(Guid id)
        {
            return this._Conditions.Where(p => p.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 移除一组条件
        /// </summary>
        /// <param name="id">组Id</param>
        public void RemoveConditionCollectionById(Guid id)
        {
            var cc = this.Conditions.Where(p => p.Id == id).FirstOrDefault();
            if (null != cc)
            {
                this._Conditions.Remove(cc);
            }
        }

        /// <summary>
        /// 快速添加一个表达式
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>新增的表达式组，组中只有condition一个表达式</returns>
        public IWfMatrixConditionGroup AddConditon(IWfMatrixCondition condition)
        {
            var maxSort = this._Conditions.Max(p => p.Sort);

            IWfMatrixConditionGroup cc = new WfMatrixConditionCollection();
            cc.Sort = maxSort + 1;
            cc.Id = Guid.NewGuid();
            cc.Add(condition);

            this._Conditions.Add(cc);

            return cc;
        }

        /// <summary>
        /// 解散一个组
        /// </summary>
        /// <param name="id">组Id</param>
        public void DismissConditionCollection(Guid id)
        {
            var cc = this.Conditions.Where(p => p.Id == id).FirstOrDefault();

            if (null != cc)
            {
                int tmpSort = cc.Sort;
                this._Conditions.Remove(cc);

                cc.ToList().ForEach(p =>
                {
                    IWfMatrixConditionGroup singleCollection = new WfMatrixConditionCollection();
                    singleCollection.Sort = tmpSort;
                    singleCollection.Id = Guid.NewGuid();
                    singleCollection.Add(p);
                    this._Conditions.Add(singleCollection);
                });

                int decrease = tmpSort;
                var sortedConditions = this._Conditions.OrderBy(p => p.Sort).ToList();
                for (int i = 0; i < sortedConditions.Count; i++)
                {
                    if (sortedConditions[i].Sort == tmpSort)
                    {
                        sortedConditions[i].Sort = sortedConditions[i].Sort + (tmpSort - decrease);
                        decrease--;
                    }
                    else if (sortedConditions[i].Sort > tmpSort)
                    {
                        sortedConditions[i].Sort = sortedConditions[i].Sort + (tmpSort - 1);
                    }
                }
            }
        }
        */
    }
}
