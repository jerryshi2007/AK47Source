using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 特权级别
    /// </summary>
    public enum DCTPrivilegeLevel
    {
        /// <summary>
        /// 高特权操作
        /// </summary>
        High = 0,
        /// <summary>
        /// 默认特权操作
        /// </summary>
        Default = 1
    }
}