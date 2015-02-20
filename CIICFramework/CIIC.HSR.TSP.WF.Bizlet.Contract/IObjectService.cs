using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 对象详细接口
    /// </summary>
    public interface IObjectService : IRuntime
    {
        /// <summary>
        /// 获取对象的详细信息
        /// </summary>
        /// <param name="objIds">对象的Id列表</param>
        /// <param name="recursive">是否</param>
        /// <returns>对象详细信息列表</returns>
        DataSet GetObjectDetail(List<string> objIds);
        /// <summary>
        /// 获取子对象
        /// </summary>
        /// <param name="objIds">父对象Id列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有的子对象</returns>
        DataSet GetObjectChildren(List<string> parentIds, bool recursive);
        /// <summary>
        /// 搜索对象
        /// </summary>
        /// <param name="parentIds">父对象Id列表，为空则从所有节点查询</param>
        /// <param name="objName">对象的名称，如组织名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有符合条件的对象</returns>
        DataSet SearchObject(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows);
    }
}
