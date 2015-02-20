#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CacheItemBase.cs
// Remark	��	CacheItem�Ļ���
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// 1.1			���		20080725		�޸�
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// CacheItem�Ļ���
    /// </summary>
    public abstract class CacheItemBase : IDisposable
    {
        private readonly CacheQueueBase cacheQueue;

        //���CacheItem��ص�Dependency,���ж�CacheItem�Ĺ���
        private DependencyBase dependency = null;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="cacheQueueBase"></param>
        public CacheItemBase(CacheQueueBase cacheQueueBase)
        {
            this.cacheQueue = cacheQueueBase;
        }

        /// <summary>
        /// ��Cache���������Cache��
        /// </summary>
        public void RemoveCacheItem()
        {
            this.cacheQueue.RemoveItem(this);
        }

        //Ϊ��CacheDependency�ܹ���CacheItem��ȡ��������Queueʵ������Ӵ˹�������
        //2008��7�£�������

        /// <summary>
        /// ��ǰCache�������ڵ�CacheQueue��
        /// </summary>
        public CacheQueueBase Queue
        {
            get
            {
                return this.cacheQueue;
            }
        }

        /// <summary>
        /// ���ԣ���ȡ���������CacheItem�������Dependency
        /// </summary>
        public DependencyBase Dependency
        {
            get { return this.dependency; }
            set { this.dependency = value; }
        }

        /// <summary>
        /// ����CacheItem����Ϊ�������
        /// </summary>
        public void SetChanged()
        {
            if (this.dependency != null)
                this.dependency.SetChanged();
        }
        #region IDisposable ��Ա

        /// <summary>
        /// ʵ��IDisposable�ӿ�
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Dependency != null)
                    this.Dependency.Dispose();
            }
        }

        #endregion

        //Ϊ��CacheDependency�ܹ���CacheItem��ȡ��Key��Value����Ӵ��鷽����
        //2008��7�£�������

        /// <summary>
        /// �õ�CacheItem��Ӧ��Key��Value
        /// </summary>
        /// <returns>���CacheItem��Key��Value����Ҫ����������</returns>
        public abstract KeyValuePair<object, object> GetKeyValue();

        //Ϊ��CacheDependency�ܹ�ΪCacheItem����Value����Ӵ��鷽����
        //2008��7�£�������

        /// <summary>
        /// ����CacheItem��ֵ
        /// </summary>
        /// <param name="value">ΪCacheItem����Value</param>
        public abstract void SetValue(object value);
    }
}
