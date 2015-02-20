#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	AbsoluteTimeDependency.cs
// Remark	：	绝对时间依赖
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 绝对时间依赖，客户端代码在初始化此类的对象时，
    /// 需要提供一个本地的绝对时间作为过期截止时间，当当前时间超过预先设定的过期时间时，
    /// 认为与此依赖项相关的Cache 项过期。
    /// </summary>
    public sealed class AbsoluteTimeDependency : DependencyBase
    {
        private DateTime expiredUtcTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expiredTime">依赖项的过期时间</param>
        /// <remarks>构造函数</remarks>
        public AbsoluteTimeDependency(DateTime expiredTime)
        {
            //如果传入的参数为DateTime.MinValue的话，不能进行UTC时间的转化
            //否则会导致时间增加8小时的错误
            if(expiredTime.Equals(DateTime.MinValue))
                this.expiredUtcTime = expiredTime;
            else
                this.expiredUtcTime = expiredTime.ToUniversalTime();

            //初始化时，将Cache项的最后修改时间和最后访问时间设置为当前时间
            this.UtcLastModified = DateTime.UtcNow;
            this.UtcLastAccessTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 属性，获取过期时间的UTC时间值
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\AbsoluteTimeDependencyTest.cs" region="ExpiredUtcTimeTest" lang="cs" title="过期时间的UTC时间属性" />
        /// </remarks>
        public DateTime ExpiredUtcTime
        {
            get { return this.expiredUtcTime; }
        }

        /// <summary>
        /// 属性，获取过期时间的本地时间值
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\AbsoluteTimeDependencyTest.cs" region="ExpiredTimeTest" lang="cs" title="过期时间的本地时间属性" />
        /// </remarks>
        public DateTime ExpiredTime
        {
            get 
            {
                if (this.expiredUtcTime.Equals(DateTime.MinValue))
                    return this.expiredUtcTime;
                return this.expiredUtcTime.ToLocalTime(); 
            }
        }

        /// <summary>
        /// 属性，获取本Dependency是否过期
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\AbsoluteTimeDependencyTest.cs" region="HasChangedTest" lang="cs" title="获取本Dependency是否过期" />
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return this.expiredUtcTime <= DateTime.UtcNow;
            }
        }
    }
}
