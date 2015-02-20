#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CacheItem.cs
// Remark	��	����ڲ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// ����ڲ�������,��װ�洢�û��ṩ��Valueֵ,�Լ���Ӧ��Dependency
    /// ����ʵ����IDisposable
    /// </summary>
    /// <typeparam name="TValue">ֵ����</typeparam>
    /// <typeparam name="TKey" >������</typeparam>
    internal class CacheItem<TKey, TValue> : CacheItemBase
    {
        private TKey key;

        public TKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        private TValue tValue;

        /// <summary>
        /// ����,��ȡ������CacheItem��value
        /// </summary>
        public TValue Value
        {
            get { return this.tValue; }
            set { this.tValue = value; }
        }

        /// <summary>
        /// ���캯������ʼ��û��Dependency��CacheItem
        /// </summary>
        /// <param name="tKey">��ʼ��CacheItem��Keyֵ</param>
        /// <param name="data">��ʼ��CacheItem��Valueֵ</param>
        /// <param name="cacheQueue">��CacheItem������CacheQueue������</param>
        public CacheItem(TKey tKey, TValue data, CacheQueueBase cacheQueue)
            : base(cacheQueue)
        {
            this.key = tKey;
            this.tValue = data;
        }

        /// <summary>
        /// ���캯������ʼ������Dependency��CacheItem
        /// </summary>
		/// <param name="tKey">��ʼ��CacheItem�ļ�ֵ</param>
        /// <param name="data">CacheItem��Value</param>
		/// <param name="dependencyBase">���CacheItem��ص�Dependency�������жϹ���</param>
        /// <param name="cacheQueue"> ��CacheItem������CacheQueue������</param>
		public CacheItem(TKey tKey, TValue data, DependencyBase dependencyBase, CacheQueueBase cacheQueue)
            : base(cacheQueue)
        {
            this.key = tKey;
            this.tValue = data;

			if (dependencyBase != null)
			{
				dependencyBase.CacheItem = this;
				dependencyBase.CacheItemBinded();
			}

			this.Dependency = dependencyBase;
        }

		/// <summary>
		/// �õ�CacheItem��KeyValueֵ
		/// </summary>
		/// <returns></returns>
		public override KeyValuePair<object, object> GetKeyValue()
		{
			return new KeyValuePair<object, object>(this.key, this.tValue);
		}

		/// <summary>
		/// ����CacheItem��ֵ
		/// </summary>
		/// <param name="value">����ֵ</param>
		public override void SetValue(object value)
		{
			this.tValue = (TValue)value;
			UpdateDependencyLastModifyTime();
		}

		/// <summary>
		/// ת����Cache����Ϣ��
		/// </summary>
		/// <returns></returns>
		public CacheItemInfo ToCacheItemInfo()
		{
			CacheItemInfo itemInfo = new CacheItemInfo();

			if (this.key != null)
				itemInfo.Key = this.key.ToString();
			else
				itemInfo.Key = "(null)";

			if (this.tValue != null)
				itemInfo.Value = this.tValue.ToString();
			else
				itemInfo.Value = "(null)";

			return itemInfo;
		}

		//����Cache�������޸�ʱ���������ʱ��
		internal void UpdateDependencyLastModifyTime()
		{
			if (this.Dependency != null)
			{
				DateTime now = DateTime.UtcNow;

				this.Dependency.UtcLastModified = now;
				this.Dependency.UtcLastAccessTime = now;
			}
		}
	}
}
