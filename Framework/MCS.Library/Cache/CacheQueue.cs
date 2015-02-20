#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	CacheQueue.cs
// Remark	��	Cache������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Caching
{
    /// <summary>
    /// һ����Cache�࣬�ڲ�ͨ��LRU�㷨ʵ����һ��Cache��Ķ��У�
    /// ��Cache�����������Ԥ���趨��Cache����ʱ�����Զ��Ѷ���β����Cache�����
    /// �û���ʹ�ô�Cacheʱ��Ҫ�Ӵ�������
    /// </summary>
    /// <typeparam name="TKey">��ֵ����</typeparam>
    /// <typeparam name="TValue">ֵ����</typeparam>
    public class CacheQueue<TKey, TValue> : CacheQueueBase, IScavenge
    {
        /// <summary>
        /// Cache�����ʱ��ί�ж���
        /// </summary>
        /// <param name="cache">Cache����</param>
        /// <param name="key">��ֵ</param>
        /// <returns>�µ�Cache��</returns>
        public delegate TValue CacheItemNotExistsAction(CacheQueue<TKey, TValue> cache, TKey key);

        /// <summary>
        /// ����LRU�㷨����Cache����ֵ�
        /// </summary>
        private LruDictionary<TKey, CacheItem<TKey, TValue>> innerDictionary;

        private readonly ReaderWriterLock rWLock = new ReaderWriterLock();
        private readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(100);
        private readonly bool overrideExistsItem = true;
        private readonly object syncRoot = new object();
        private readonly int defaultMaxLength = -1;

        #region ���췽��
        /// <summary>
        /// ���캯����û������CacheQueue��������С����ʹ��Ĭ��ֵ100
        /// </summary>
        protected CacheQueue()
            : base()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="overrideExists">Add Cache��ʱ���Ƿ񸲸����е�����</param>
        protected CacheQueue(bool overrideExists)
            : this()
        {
            this.overrideExistsItem = overrideExists;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="overrideExists"></param>
        /// <param name="instanceName"></param>
        protected CacheQueue(bool overrideExists, string instanceName)
            : base(instanceName)
        {
            this.overrideExistsItem = overrideExists;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="overrideExists"></param>
        /// <param name="instanceName"></param>
        /// <param name="maxLength"></param>
        protected CacheQueue(bool overrideExists, string instanceName, int maxLength)
            : base(instanceName)
        {
            this.overrideExistsItem = overrideExists;
            this.defaultMaxLength = maxLength;
        }
        #endregion ���췽��

        #region ��������

        /// <summary>
        /// ���ԣ���ȡCacheQueue���������
        /// </summary>
        public int MaxLength
        {
            get
            {
                int maxLength = this.defaultMaxLength;

                if (maxLength == -1)
                {
                    QueueSetting qs = CacheSettingsSection.GetConfig().QueueSettings[this.GetType()];

                    if (qs != null)
                        maxLength = qs.QueueLength;
                    else
                        maxLength = CacheSettingsSection.GetConfig().DefaultQueueLength;
                }

                return maxLength;
            }
        }
        #endregion ��������

        /// <summary>
        /// ��CacheQueue������һCache��ֵ�ԣ������Ӧ��key�Ѿ����ڣ����׳��쳣
        /// ���ֹ��췽�������Dependency�����Դ�����Cache�����ڣ�ֻ���ܵ�CacheQueue
        /// �ĳ��ȳ���Ԥ���趨ʱ���ſ��ܱ������
        /// </summary>
        /// <param name="key">��</param>
        /// <param name="data">ֵ</param>
        /// <returns>ֵ</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCacheItem��" />
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
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCacheItem��" />
        /// </remarks>
        public TValue Add(TKey key, TValue data, DependencyBase dependency)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                InnerDictionary.MaxLength = this.MaxLength;

                //��ɾ���Ѿ����ڶ��ҹ��ڵ�Cache��
                if (InnerDictionary.ContainsKey(key) &&
                    ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency != null &&
                    ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency.HasChanged)
                    this.Remove(key);

                CacheItem<TKey, TValue> item = new CacheItem<TKey, TValue>(key, data, dependency, this);

                if (dependency != null)
                {
                    dependency.UtcLastModified = DateTime.UtcNow;
                    dependency.UtcLastAccessTime = DateTime.UtcNow;
                }

                if (this.overrideExistsItem)
                    InnerDictionary[key] = item;
                else
                    InnerDictionary.Add(key, item);

                this.Counters.EntriesCounter.RawValue = InnerDictionary.Count;

                return data;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ���ԣ���ȡCacheQueue�еĴ洢��Cache�������
        /// </summary>
        public override int Count
        {
            get
            {
                return InnerDictionary.Count;
            }
        }

        /// <summary>
        /// ͨ��Cache���key��ȡCache��Value��������
        /// </summary>
        /// <param name="key">cache��key</param>
        /// <returns>cache��Value</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="ͨ��Cache���key��ȡCache��Value" />
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                CacheItem<TKey, TValue> item = null;
                this.TotalCounters.HitRatioBaseCounter.Increment();
                this.Counters.HitRatioBaseCounter.Increment();

                try
                {
                    this.rWLock.AcquireReaderLock(this.lockTimeout);
                    try
                    {

                        item = InnerDictionary[key];
                    }
                    finally
                    {
                        this.rWLock.ReleaseReaderLock();
                    }

                    this.rWLock.AcquireWriterLock(this.lockTimeout);
                    try
                    {
                        //���Cache��������׳��쳣
                        this.CheckDependencyChanged(key, item);

                        //����cache���������ʱ��
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

                    return InnerDictionary[key].Value;
                }
                catch (System.Exception)
                {
                    this.TotalCounters.MissesCounter.Increment();
                    this.Counters.MissesCounter.Increment();
                    throw;
                }
            }
            set
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    CacheItem<TKey, TValue> item;

                    if (InnerDictionary.TryGetValue(key, out item) == false)
                    {
                        this.Add(key, value);
                        item = InnerDictionary[key];
                    }
                    else
                        item.Value = value;

                    item.UpdateDependencyLastModifyTime();
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
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
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="ͨ��key����ȡCache���value" />
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
                //������ʱ��Ҳ���޸��ֵ䣨�������򣩣������Ҫ��ס�ֵ䣬���в�������
                lock (InnerDictionary.SyncRoot)
                {
                    result = InnerDictionary.TryGetValue(key, out item);
                }
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
                    //�ж�cache���Ƿ����
                    if (this.GetDependencyChanged(key, item))
                        result = false;
                    else
                    {
                        data = item.Value;
                        //�޸�Cache���������ʱ��
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
        public TValue GetOrAddNewValue(TKey key, CacheItemNotExistsAction action)
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
        /// <param name="key">����Ψһ��ʶ</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCacheItem��" />
        /// </remarks>
        public void Remove(TKey key)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                CacheItem<TKey, TValue> item;

                if (InnerDictionary.TryGetValue(key, out item))
                    this.InnerRemove(key, item);

                this.Counters.EntriesCounter.RawValue = InnerDictionary.Count;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// ���ػ��෽����ɾ�������CacheItem
        /// </summary>
        /// <param name="cacheItem">������ж���</param>
        internal protected override void RemoveItem(CacheItemBase cacheItem)
        {
            this.Remove(((CacheItem<TKey, TValue>)cacheItem).Key);
        }

        /// <summary>
        /// �ж�CacheQueue���Ƿ����key����Cache��
        /// </summary>
        /// <param name="key">��ѯ��cache��ļ�ֵ</param>
        /// <returns>��������˼�ֵ������true�����򷵻�false</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCacheItem��" />
        /// </remarks>
        public bool ContainsKey(TKey key)
        {
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                bool result = ((InnerDictionary.ContainsKey(key) &&
                            ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency == null) ||
                                (InnerDictionary.ContainsKey(key) &&
                                ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency != null
                                && !((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency.HasChanged));

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
        /// �������CacheQueue��ɾ��CacheQueue�����е�Cache��
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="���ӡ��Ƴ�����ȡCacheItem��" />
        /// </remarks>
        public override void Clear()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in InnerDictionary)
                    kp.Value.Dispose();

                InnerDictionary.Clear();
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
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in InnerDictionary)
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
        private void CheckDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            if (GetDependencyChanged(key, item))
                throw new DependencyChangedException(string.Format(Resource.DependencyChanged, key, item.Dependency));
        }

        /// <summary>
        /// ɾ��Cache��
        /// </summary>
        /// <param name="key">Cache���ֵ</param>
        /// <param name="item">Cache��</param>
        private void InnerRemove(TKey key, CacheItem<TKey, TValue> item)
        {
            InnerDictionary.Remove(key);
            item.Dispose();
        }

        private LruDictionary<TKey, CacheItem<TKey, TValue>> InnerDictionary
        {
            get
            {
                //����һ�����̰߳�ȫ��Lru�ֵ䣬�̰߳�ȫ�����CacheQueue����
                if (this.innerDictionary == null)
                    this.innerDictionary = new LruDictionary<TKey, CacheItem<TKey, TValue>>(MaxLength);

                return this.innerDictionary;
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
    }
}
