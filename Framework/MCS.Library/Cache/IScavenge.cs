#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	IScavenge.cs
// Remark	：	CacheQueue清理接口类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 接口，各中CacheQueue通过实现此接口完成自身的清理工作
    /// </summary>
    public interface IScavenge
    {
        /// <summary>
        /// Cache对列清理方法
        /// </summary>
        void DoScavenging();
    }
}
