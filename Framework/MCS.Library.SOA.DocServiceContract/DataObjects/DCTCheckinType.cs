using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 签入类型
    /// </summary>
    /// 
     [Serializable]
    public enum DCTCheckinType
    {
        /// <summary>
        /// 次要版本签入
        /// </summary>
        MinorCheckIn = 0,
        /// <summary>
        /// 主要版本签入
        /// </summary>
        MajorCheckIn= 1,
        /// <summary>
        /// 覆盖签入
        /// </summary>
        OverwriteCheckIn=2,
    }
}