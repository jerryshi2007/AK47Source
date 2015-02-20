using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 服务创建工厂
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        private BizObject.ServiceContext _Context = null;

        public BizObject.ServiceContext Context
        {
            get { return _Context; }
            set { 
                _Context = value; 
            }
        }
        public ServiceFactory()
        {
            //默认值，无租户状态
            _Context = new BizObject.ServiceContext() { TenantCode = Consts.DefaultTenantCode };
        }
        /// <summary>
        /// 创建接口
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="interfaceType">请求的接口</param>
        /// <returns></returns>
        public T CreateService<T>() where T:class,IRuntime
        {
            //试图从IOC获取接口实例
            T service = Containers.Default.ResolveAll<T>().FirstOrDefault();
            if (null == service)
            {
                service = GetDefaultImplement<T>() as T;
            }
            service.Context = _Context;
            return service;
        }
        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        private object GetDefaultImplement<T>()
        {
            if (typeof(T).Name == typeof(ITaskOperator).Name)
            {
                return new TaskOperator();
            }
            if (typeof(T).Name == typeof(ITaskQuery).Name)
            {
                return new TaskQuery();
            }
            if (typeof(T).Name == typeof(IReaderServiceBizlet).Name)
            {
                return new ReaderServiceBizlet();
            }
            if (typeof(T).Name == typeof(ISecurityCheckServiceBizlet).Name)
            {
                return new SecurityCheckServiceBizlet();
            }
            if (typeof(T).Name == typeof(ITaskPluginBizlet).Name)
            {
                return new TaskPluginBizlet();
            }
            if (typeof(T).Name==typeof(IOguProvider).Name)
            {
                return new OguOmpProvider();
            }
            return null;
        }
    }
}
