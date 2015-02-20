using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject.Exchange
{
    /// <summary>
    /// 应用
    /// </summary>
    public class Application
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string CodeName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 排列序号，不存在可以填0
        /// </summary>
        public string SortId { get; set; }
    }
}
