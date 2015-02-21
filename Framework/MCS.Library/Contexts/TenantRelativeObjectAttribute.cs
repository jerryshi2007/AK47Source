using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Core
{
    /// <summary>
    /// 为对象添加此属性时，会在Adapter的基类中添加这个字段的处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TenantRelativeObjectAttribute : Attribute
    {
        private string _TenantCodeFieldName = "TENANT_CODE";

        /// <summary>
        /// 构造方法
        /// </summary>
        public TenantRelativeObjectAttribute()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tenantCodeFieldName">租户编码的所对应的数据库字段，默认值是TENANT_CODE</param>
        public TenantRelativeObjectAttribute(string tenantCodeFieldName)
        {
            this._TenantCodeFieldName = tenantCodeFieldName;
        }

        /// <summary>
        /// 租户编码的所对应的数据库字段，默认值是TENANT_CODE
        /// </summary>
        public string TenantCodeFieldName
        {
            get
            {
                return this._TenantCodeFieldName;
            }
            set
            {
                this._TenantCodeFieldName = value;
            }
        }
    }
}
