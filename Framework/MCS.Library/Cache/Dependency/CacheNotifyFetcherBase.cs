using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// 消息通知数据获取器的基类
    /// </summary>
    public abstract class CacheNotifyFetcherBase : IDisposable
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual TimeSpan GetInterval()
        {
            return TimeSpan.Zero;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public abstract CacheNotifyData[] GetData();

        #region IDisposable Members
        /// <summary>
        /// 释放资源
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
}
