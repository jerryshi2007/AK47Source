using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public class TenantHelper
    {
        /// <summary>
        /// 获取默认租户编码，当参数TenantCode为空时，返回默认值；否则，返回TenantCode
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public static string GetDefaultTenant(string tenantCode)
        {
            if (!string.IsNullOrEmpty(tenantCode))
            {
                return tenantCode;
            }

            return Consts.DefaultTenantCode;
        }
    }
}
