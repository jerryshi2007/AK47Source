using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.PermissionManager.Storage;
using MCS.Library.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.ObjectDetail
{
    /// <summary>
    /// 对象服务构造器
    /// </summary>
    public class ObjectServiceFactory
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="objectType">欲创建的类型</param>
        /// <returns>服务对象</returns>
        public IObjectService Create(string objectType)
        {
            IObjectService iObjectService = null;
            if (objectType.Equals(ObjectType.GROUPS, StringComparison.CurrentCultureIgnoreCase)
                || objectType.Equals(ObjectType.GROUPSName, StringComparison.CurrentCultureIgnoreCase))
            {
                iObjectService= new GroupService();
            }
            else if (objectType.Equals(ObjectType.ORGANIZATIONS, StringComparison.CurrentCultureIgnoreCase)
                ||(objectType.Equals(ObjectType.ORGANIZATIONSName, StringComparison.CurrentCultureIgnoreCase)))
            {
                iObjectService =new OrgService();
            } 
            else if (objectType.Equals(ObjectType.USERS, StringComparison.CurrentCultureIgnoreCase)
                || objectType.Equals(ObjectType.USERSName, StringComparison.CurrentCultureIgnoreCase))
            {
                iObjectService =new UserService();
            }
            return SetRuntime(iObjectService);
        }
        /// <summary>
        /// 创建权限管理服务
        /// </summary>
        /// <returns>权限管理服务接口</returns>
        public IARPService CreateARPService()
        {
            return SetRuntime(new ARPService());
        }
        /// <summary>
        /// 获取权限管理接口
        /// </summary>
        /// <returns>权限管理接口</returns>
        public IOguProvider CreatOguProvider()
        {
            TenantSection tenantSection = ConfigurationManager.GetSection("tenantSetting") as TenantSection;
            string registerKey = tenantSection.Tenants.GetEffectiveElement().Name;
            IOguProvider oguProvider = Containers.Global.Singleton.Resolve<IOguProvider>(registerKey);
            return oguProvider;
        }
        /// <summary>
        /// 设置接口上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        public T SetRuntime<T>(T service) where T:IRuntime
        {
            string tenantCode = string.Empty;
            if (SimpleRequestSoapMessage.Current.Context.ContainsKey("TenantCode"))
            {
                tenantCode = SimpleRequestSoapMessage.Current.Context["TenantCode"].ToString();
            }
            service.Context = new BizObject.ServiceContext(){ TenantCode=tenantCode };
            return service;
        }
    }
}
