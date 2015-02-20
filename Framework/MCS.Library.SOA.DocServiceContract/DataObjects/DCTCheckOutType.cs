using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 签出类型
    /// </summary>
     [Serializable]
    public enum DCTCheckOutType
    {
        /// <summary>
        /// 在线签出
        /// </summary>
        Online = 0,
        /// <summary>
        /// 离线签出
        /// </summary>
        OffLine = 1,
        /// <summary>
        /// 没有签出
        /// </summary>
        None = 2
    }
}