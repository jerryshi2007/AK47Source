using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 权限结构构建
    /// </summary>
    public class PermissionStructureBuilder:ARPStructureBuilderBase
    {
        /// <summary>
        /// 权限结构描述
        /// </summary>
        /// <returns>权限结构描述</returns>
        protected override List<Column> GetObjectColumns()
        {
            //权限无扩展字段
            return new List<Column>();
        }
    }
}
