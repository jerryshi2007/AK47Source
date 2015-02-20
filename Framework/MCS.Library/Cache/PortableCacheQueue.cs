#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	PortableCacheQueue.cs
// Remark	��	Portable���������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Ϊһ����Cache�࣬��CacheQueue��ͬ���ǣ�PortableCacheQueue�ڲ�û��ʵ��LRU�㷨��
    /// ����������СҲ�������ơ��û���ʹ�ô�Cacheʱͬ����Ҫ�Ӵ�������һ���࣬���ֹ�ע�ᣬ
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PortableCacheQueue<TKey, TValue> : CacheQueueBase, IScavenge
    {
        /// <summary>
        /// Cache�����ʱ��ί�ж���
        /// </summary>
        /// <param name="cache">Cache����</param>
        /// <param name="key">��ֵ</param>
        /// <returns>�µ�Cache��</returns>
        public delegate TValue PortableCacheItemNotExistsAction(PortableCacheQueue<TKey, TValue> cache, TKey key);

        private readonly Dictionary<TKey, CacheItem<TKey, TValue>> innerDictionary = new Dictionary<TKey, CacheItem<TKey, TValue>>();

        private readonly ReaderWriterLock rWLock = new ReaderWriterLock();
        private readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(100);
        private readonly bool overrideExistsItem = true;
        private readonly object syncRoot = new object();

        /// <summary>
        /// ���췽��
        /// </summary>
        protected PortableCacheQueue()
            : base()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="overrideExists">Add Cache��ʱ���Ƿ񸲸����е�����</param>
        protected PortableCacheQueue(bool overrideExists)
        {
            this.overrideExistsItem = overrideExists;
        }

        /// <summary>
        /// ��CacheQueue������һCache��ֵ�ԣ������Ӧ��key�Ѿ����ڣ����׳��쳣
        /// ���ֹ��췽�������Dependency�����Դ�����Cache�����ڣ�ֻ���ܵ�CacheQueue
        /// �ĳ��ȳ���Ԥ���趨ʱ���ſ��ܱ������
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="data">ֵ</param>
        /// <returns>ֵ</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCahceItem��" />
        /// </remarks>
        public TValue Add(TKey key, TValue data)
        {
            this.Add(key, data, null);

            return data;
        }

        /// <summary>
        /// ��CacheQueue������һ���й���Dependency��Cache������Ӧ��key�Ѿ����ڣ����׳��쳣
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="data">ֵ</param>
        /// <param name="dependency">���������ʱ������������ʱ���������ļ�������������</param>
        /// <returns>ֵ</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCahceItem��" />
        /// </remarks>
        public TValue Add(TKey key, TValue data, DependencyBase dependency)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);

            try
            {
                //ɾ���Ѿ����ڶ��ҹ��ڵ�Cache��
                if (this.innerDictionary.ContainsKey(key) &&
                    ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency != null &&
                    ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency.HasChanged)
                    this.Remove(key);

                CacheItem<TKey, TValue> item = new CacheItem<TKey, TValue>(key, data, dependency, this);

                if (dependency != null)
                {
                    dependency.UtcLastModified = DateTime.UtcNow;
                    dependency.UtcLastAccessTime = DateTime.UtcNow;
                }

                if (this.overrideExistsItem)
                    this.innerDictionary[key] = item;
                else
                    this.innerDictionary.Add(key, item);

                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;

                return data;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ���ԣ���ȡCacheQueue���������
        /// </summary>
        public override int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        /// <summary>
        /// ͨ��Cache���key��ȡCache��Value��������
        /// </summary>
        /// <param name="key">cache��key</param>
        /// <returns>cache��Value</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="ͨ��Cache���key��ȡCache��Value" />
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                CacheItem<TKey, TValue> item = null;
                this.TotalCounters.HitRatioBaseCounter.Increment();
                this.Counters.HitRatioBaseCounter.Increment();

                this.rWLock.AcquireReaderLock(this.lockTimeout);
                try
                {
                    item = this.innerDictionary[key];
                }
                finally
                {
                    this.rWLock.ReleaseReaderLock();
                }

                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    this.CheckDependencyChanged(key, item);

                    if (item.Dependency != null)
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }

                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();

                return item.Value;
            }
            set
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    CacheItem<TKey, TValue> item = this.innerDictionary[key];

                    item.Value = value;

                    //����Cache�������޸�ʱ���������ʱ��
                    if (item.Dependency != null)
                    {
                        item.Dependency.UtcLastModified = DateTime.UtcNow;
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// �ж�PortableCacheQueue���Ƿ����key����Cache��
        /// </summary>
        /// <param name="key">��ѯ��cache��ļ�ֵ</param>
        /// <returns>��������˼�ֵ������true�����򷵻�false</returns>
        public bool ContainsKey(TKey key)
        {
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                bool result = ((this.innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency == null) ||
                        (this.innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency != null &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency.HasChanged == false));

                if (result)
                {
                    this.TotalCounters.HitsCounter.Increment();
                    this.TotalCounters.HitRatioCounter.Increment();
                    this.Counters.HitsCounter.Increment();
                    this.Counters.HitRatioCounter.Increment();
                }
                else
                {
                    this.TotalCounters.MissesCounter.Increment();
                    this.Counters.MissesCounter.Increment();
                }

                return result;
            }
            finally
            {
                this.rWLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// ͨ��key����ȡCache���value�������Ӧ��cache����ڵĻ�
        /// ��cache���value��Ϊ������������ظ��ͻ��˴���
        /// </summary>
        /// <param name="key">cache���key</param>
        /// <param name="data">cache���value</param>
        /// <returns>���CacheQueue�а�����Cache��򷵻�true�����򷵻�false</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="ͨ��key����ȡCache���value" />
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue data)
        {
            data = default(TValue);
            CacheItem<TKey, TValue> item;
            bool result;

            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                result = this.innerDictionary.TryGetValue(key, out item);
            }
            finally
            {
                this.rWLock.ReleaseReaderLock();
            }

            if (result)
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    if (this.GetDependencyChanged(key, item))
                        result = false;
                    else
                    {
                        data = item.Value;
                        if (item.Dependency != null)
                            item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
            }

            if (result)
            {
                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();
            }
            else
            {
                this.TotalCounters.MissesCounter.Increment();
                this.Counters.MissesCounter.Increment();
            }

            return result;
        }

        /// <summary>
        /// ��Cache�ж�ȡCache���������ڣ������action
        /// </summary>
        /// <param name="key">��ֵ</param>
        /// <param name="action">������ʱ�Ļص�</param>
        /// <returns>Cache���ֵ</returns>
        public TValue GetOrAddNewValue(TKey key, PortableCacheItemNotExistsAction action)
        {
            TValue result;

            if (TryGetValue(key, out result) == false)
            {
                lock (this.syncRoot)
                {
                    if (TryGetValue(key, out result) == false)
                        result = action(this, key);
                }
            }

            return result;
        }

        /// <summary>
        /// ͨ��key��ɾ��һCache��
        /// </summary>
        /// <param name="key">��</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCahceItem��" />
        /// </remarks>
        public void Remove(TKey key)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                CacheItem<TKey, TValue> item;

                if (this.innerDictionary.TryGetValue(key, out item))
                    this.InnerRemove(key, item);

                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ���ػ��෽����ɾ�������CacheItem
        /// </summary>
        /// <param name="cacheItem"></param>
        internal protected override void RemoveItem(CacheItemBase cacheItem)
        {
            this.Remove(((CacheItem<TKey, TValue>)cacheItem).Key);
        }

        /// <summary>
        /// �������CacheQueue��ɾ��CacheQueue�����е�Cache��
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCahceItem��" />
        /// </remarks>
        public override void Clear()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    kp.Value.Dispose();

                this.innerDictionary.Clear();
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ȫ��������
        /// </summary>
        public override void SetChanged()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    kp.Value.SetChanged();
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ������������CacheQueue�й��ڵ�cache��
        /// </summary>
        public void DoScavenging()
        {
            List<KeyValuePair<TKey, CacheItem<TKey, TValue>>> keysToRemove = new List<KeyValuePair<TKey, CacheItem<TKey, TValue>>>();

            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    if (kp.Value.Dependency != null && kp.Value.Dependency.HasChanged)
                        keysToRemove.Add(kp);

                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in keysToRemove)
                    this.InnerRemove(kp.Key, kp.Value);
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// �õ����������Ϣ
        /// </summary>
        /// <returns></returns>
        public override CacheItemInfoCollection GetAllItemsInfo()
        {
            CacheItemInfoCollection result = new CacheItemInfoCollection();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                {
                    CacheItemInfo itemInfo = new CacheItemInfo();

                    result.Add(kp.Value.ToCacheItemInfo());
                }
            }
            finally
            {
                this.rWLock.ReleaseReaderLock();
            }

            return result;
        }

        /// <summary>
        /// ɾ��Cache��
        /// </summary>
        /// <param name="key">Cache���ֵ</param>
        /// <param name="item">Cache��</param>
        private void InnerRemove(TKey key, CacheItem<TKey, TValue> item)
        {
            this.innerDictionary.Remove(key);
            item.Dispose();
        }

        private bool GetDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            bool result = false;

            if (item.Dependency != null && item.Dependency.HasChanged)
            {
                result = true;
                this.InnerRemove(key, item);
            }

            return result;
        }

        /// <summary>
        /// �ж�һCache���Ƿ����
        /// </summary>
        /// <param name="key">Cache��ļ�ֵ</param>
        /// <param name="item">Cache��</param>
        /// <returns>���Cache����ڣ�����true��������ɾ�������򷵻�false</returns>
        private void CheckDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            if (GetDependencyChanged(key, item))
                throw new DependencyChangedException(string.Format(Resource.DependencyChanged, key, item.Dependency));
        }
    }
}
