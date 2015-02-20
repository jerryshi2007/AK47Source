#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ObjectCacheQueue.cs
// Remark	：	通用类型的CacheQueue类
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
    /// 用于存放通用类型的CacheQueue
    /// </summary>
    public sealed class ObjectCacheQueue : CacheQueue<object, object>
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        private static readonly ObjectCacheQueue instance = CacheManager.GetInstance<ObjectCacheQueue>();
       
		/// <summary>
		/// 获取实例
		/// </summary>
		public static ObjectCacheQueue Instance
		{
			get
			{
				return ObjectCacheQueue.instance;
			}
		}

        //实现SingleTon模式
        private ObjectCacheQueue()
        {
        }
    }
}
