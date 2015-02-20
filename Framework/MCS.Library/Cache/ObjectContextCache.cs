#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ObjectContextCache.cs
// Remark	：	通用类型的上下文中缓存类
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
    /// 存放Key和Value都是object的ContextCache
    /// </summary>
    public sealed class ObjectContextCache : ContextCacheQueueBase<object, object>
    {
        /// <summary>
        /// ObjectContextCache的实例，此处必须是属性，动态计算
        /// </summary>
        public static ObjectContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<ObjectContextCache>();
            }
        }

        private ObjectContextCache()
        {
        }
    }
}
