using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Library.Core
{
    /// <summary>
    /// 租户涉及到的扩展方法
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// 租户代码的参数名称
        /// </summary>
        public const string TenantCodeParamName = "tenantCode";

        /// <summary>
        /// 从HttpContext中获取租户上下文。
        /// 优先从Header中读取，没有再从Rquest QueryString中读取
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTenantCode(this HttpContext context)
        {
            string tenantCode = string.Empty;

            if (context != null)
            {
                tenantCode = context.Request.Headers.GetValue(TenantCodeParamName, string.Empty);

                if (tenantCode.IsNullOrEmpty())
                    tenantCode = context.Request.QueryString.GetValue(TenantCodeParamName, string.Empty);
                else
                    tenantCode = HttpUtility.UrlDecode(tenantCode);
            }

            return tenantCode;
        }
    }
}
