using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 列描述
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 列明
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public Type CType { get; set; }
    }
}
