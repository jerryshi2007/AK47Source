using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    /// <summary>
    /// 通用组织、组和人员结构
    /// </summary>
    public class OGUCommonStructureBuilder:OGUStructureBuilderBase
    {
        /// <summary>
        /// 通用结构描述
        /// </summary>
        /// <returns>通用结构描述</returns>
        protected override List<Column> GetObjectColumns()
        {
            return new List<Column>();
        }
    }
}
