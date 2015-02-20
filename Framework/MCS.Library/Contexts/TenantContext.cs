using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Core
{
    /// <summary>
    /// 租户相关的上下文信息
    /// </summary>
    [ActionContextDescription(Key = "TenantContext")]
    public class TenantContext : ActionContextBase<TenantContext>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TenantContext()
        {
            this.InitFromSettings();
        }

        /// <summary>
        /// 租户上下文
        /// </summary>
        public string TenantCode
        {
            get;
            set;
        }

        /// <summary>
        /// 是否启用租户上下文
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        private void InitFromSettings()
        {
            TenantContextSettings settings = TenantContextSettings.GetConfig();

            this.TenantCode = settings.DefaultTenantCode;
            this.Enabled = settings.Enabled;
        }
    }
}
