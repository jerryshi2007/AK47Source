using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 服务创建器
    /// </summary>
    public interface IServiceFactory:IRuntime
    {
        /// <summary>
        /// 创建接口
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="tenantCode">租户编码</param>
        /// <returns></returns>
        T CreateService<T>() where T : class,IRuntime;
    }
}
