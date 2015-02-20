#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	DependencyBase.cs
// Remark	：	所有Dependency的抽象基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// 1.1			沈峥		20080725		修改
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 所有Dependency的抽象基类，此基类实现了IDisposable接口
    /// </summary>
    public abstract class DependencyBase : IDisposable
    {
        //Cache项的最后修改时间
        private DateTime utcLastModified;

        //Cache项的最后访问时间
        private DateTime utlLastAccessTime;

        //包含本Dependency的CacheItem的引用
        private CacheItemBase cacheItem;

        /// <summary>
        /// Dependency所依赖的CacheItem
        /// </summary>
        public CacheItemBase CacheItem
        {
            get { return this.cacheItem; }
            internal set { this.cacheItem = value; }
        }

        /// <summary>
        /// 属性,获取或设置Cache项最后修改时间的UTC时间值
        /// </summary>
        public virtual DateTime UtcLastModified
        {
            get { return this.utcLastModified; }
            set { this.utcLastModified = value; }
        }

        /// <summary>
        /// 属性,获取或设置Cache项的最后访问时间的UTC时间值
        /// </summary>
        public virtual DateTime UtcLastAccessTime
        {
            get { return this.utlLastAccessTime; }
            set { this.utlLastAccessTime = value; }
        }

        /// <summary>
        /// 属性,获取此Dependency是否过期
        /// </summary>
        public virtual bool HasChanged
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected virtual void SetChanged()
        {
        }
        //2008年7月，沈峥添加

        /// <summary>
        /// 当Dependency对象绑定到CacheItem时，会调用此方法。此方法被调用时，保证Dependency的CacheItem属性已经有值
        /// </summary>
        internal protected virtual void CacheItemBinded()
        {
        }

        #region IDisposable 成员

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }

    /// <summary>
    /// Dependency失效，导致Cache的Key访问失效所使用的异常
    /// </summary>
    [Serializable]
    public class DependencyChangedException : SystemSupportException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public DependencyChangedException()
            : base()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        public DependencyChangedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">异常对象</param>
        public DependencyChangedException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
