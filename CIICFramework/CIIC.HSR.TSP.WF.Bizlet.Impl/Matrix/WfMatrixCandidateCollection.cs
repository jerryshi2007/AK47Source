using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixCandidateCollection : List<IWfMatrixCandidate>, IWfMatrixCandidateCollection
    {
        /* 沈峥注释
        /// <summary>
        /// 删除一个动态角色
        /// </summary>
        /// <param name="id">动态角色Id</param>
        public void Remove(Guid id)
        {
            var candidate = this.Where(p => p.ID == id).FirstOrDefault();
            if (null != candidate)
            {
                this.Remove(candidate);
            }
        }

        /// <summary>
        /// 根据Id获取参数定义
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>参数定义信息</returns>
        public IWfMatrixCandidate GetById(Guid id)
        {
            return this.Where(p => p.ID == id).FirstOrDefault(); 
        }
        */

        /// <summary>
        /// 将动态角色列表转换为逗号“，”分割字符串
        /// </summary>
        /// <returns>逗号“，”分割字符串</returns>
        public string ToExpression()
        {
            StringBuilder strB = new StringBuilder(); 
            foreach (IWfMatrixCandidate candidate in this)
            {
                strB.AppendFormat(",{0}", candidate.ToExpression());
            }
            if (strB.Length > 0)
                strB.Remove(0,1);
            return strB.ToString();

            //沈峥评语，好难看的拼串
            //string displayInfo = string.Empty;


            //this.ForEach(p =>
            //{
            //    displayInfo = string.Format("{0}{1},", displayInfo, p.ToExpression());
            //});

            //displayInfo.TrimEnd(new[] { ',' });
            //return displayInfo;
        }
    }
}
