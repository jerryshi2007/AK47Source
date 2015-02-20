using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject.Exchange
{
    /// <summary>
    /// 组织
    /// </summary>
    public class Org
    {
        /// <summary>
        /// 组织的路径，以","分割的Id列表，形式：Id1,Id2,Id3...Idn
        /// </summary>
        public string AllPath { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 上级组织ID
        /// </summary>
        public string ParentOrgId { get; set; }
    }
}
