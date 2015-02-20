using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public class TaskCategory
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public string CategoryID { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 排序ID
        /// </summary>
        public int InnerSortID { get; set; } 


    }
}
