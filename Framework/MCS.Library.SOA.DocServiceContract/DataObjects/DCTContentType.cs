using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 对象类型
    /// </summary>
    [Flags]
    [Serializable]
	public enum DCTContentType
    {
		/// <summary>
		/// None
		/// </summary>
		None = 0,

        /// <summary>
        /// 文件夹
        /// </summary>
        Folder = 1,
 
		/// <summary>
        /// 文件
        /// </summary>
        File = 2,

		/// <summary>
        /// 全部
        /// </summary>
        All = 3

    }
}