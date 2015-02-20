using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class GroupStructureBuilder : OGUStructureBuilderBase
    {
        /// <summary>
        /// 角色实例列描述
        /// </summary>
        /// <returns></returns>
        protected override List<Column> GetObjectColumns()
        {
            //组无扩展字段
            return new List<Column>();
        }
    }
}
