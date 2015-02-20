#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	DependencyBase.cs
// Remark	��	����Dependency�ĳ������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// 1.1			���		20080725		�޸�
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// ����Dependency�ĳ�����࣬�˻���ʵ����IDisposable�ӿ�
    /// </summary>
    public abstract class DependencyBase : IDisposable
    {
        //Cache�������޸�ʱ��
        private DateTime utcLastModified;

        //Cache���������ʱ��
        private DateTime utlLastAccessTime;

        //������Dependency��CacheItem������
        private CacheItemBase cacheItem;

        /// <summary>
        /// Dependency��������CacheItem
        /// </summary>
        public CacheItemBase CacheItem
        {
            get { return this.cacheItem; }
            internal set { this.cacheItem = value; }
        }

        /// <summary>
        /// ����,��ȡ������Cache������޸�ʱ���UTCʱ��ֵ
        /// </summary>
        public virtual DateTime UtcLastModified
        {
            get { return this.utcLastModified; }
            set { this.utcLastModified = value; }
        }

        /// <summary>
        /// ����,��ȡ������Cache���������ʱ���UTCʱ��ֵ
        /// </summary>
        public virtual DateTime UtcLastAccessTime
        {
            get { return this.utlLastAccessTime; }
            set { this.utlLastAccessTime = value; }
        }

        /// <summary>
        /// ����,��ȡ��Dependency�Ƿ����
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
        //2008��7�£�������

        /// <summary>
        /// ��Dependency����󶨵�CacheItemʱ������ô˷������˷���������ʱ����֤Dependency��CacheItem�����Ѿ���ֵ
        /// </summary>
        internal protected virtual void CacheItemBinded()
        {
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
    /// DependencyʧЧ������Cache��Key����ʧЧ��ʹ�õ��쳣
    /// </summary>
    [Serializable]
    public class DependencyChangedException : SystemSupportException
    {
        /// <summary>
        /// ���췽��
        /// </summary>
        public DependencyChangedException()
            : base()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        public DependencyChangedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        /// <param name="innerException">�쳣����</param>
        public DependencyChangedException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
