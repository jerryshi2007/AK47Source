using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public class ServiceContext
    {
        private string _TenantCode = Consts.DefaultTenantCode;
        /// <summary>
        /// 租户编码
        /// </summary>
        public string TenantCode {
            get
            {
                if (string.IsNullOrEmpty(_TenantCode))
                {
                    return Consts.DefaultTenantCode;
                }
                return _TenantCode;
            }
            set
            {
                _TenantCode = value;
            }
        }
    }
}
