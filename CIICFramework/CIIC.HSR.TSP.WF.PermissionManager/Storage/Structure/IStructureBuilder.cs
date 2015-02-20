using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 构建器接口
    /// </summary>
    public interface IStructureBuilder
    {
        /// <summary>
        /// 创建数据结构
        /// </summary>
        /// <returns>返回创建好的数据结构</returns>
        DataSet Create();
    }
}
