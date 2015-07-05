using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 上下文字典的基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [XElementFieldSerialize(IgnoreDeserializeError = true)]
    public abstract class WfContextDictionaryBase<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, ISerializable
    {
        private const string DictionarySerializationKey = "WfContextDictionaryBase`2+_Dictionary";

        private Hashtable _Dictionary = new Hashtable(StringComparer.OrdinalIgnoreCase);

        private string _SerializedDictionary = string.Empty;

        [NonSerialized]
        private ReaderWriterLock rwLock = null;

        [NonSerialized]
        private static readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(100);

        public WfContextDictionaryBase()
        {
        }

        public WfContextDictionaryBase(SerializationInfo info, StreamingContext context)
        {
            bool dictionaryInited = false;

            try
            {
                this._SerializedDictionary = info.GetString("_SerializedDictionary");

                if (this._SerializedDictionary.IsNotEmpty())
                {
                    this._Dictionary = (Hashtable)SerializationHelper.DeserializeStringToObject(this._SerializedDictionary, SerializationFormatterType.Binary, UnknownTypeStrategyBinder.Instance);
                    dictionaryInited = true;
                }
            }
            catch (SerializationException)
            {
            }

            if (dictionaryInited == false)
            {
                try
                {
                    this._Dictionary = (Hashtable)info.GetValue(DictionarySerializationKey, typeof(Hashtable));
                }
                catch (SerializationException)
                {
                }
            }
        }

        /// <summary>
        /// 按照key访问字典元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>字典中的元素</returns>
        public TValue this[TKey key]
        {
            get
            {
                AcquireReaderLock();
                try
                {
                    return (TValue)this._Dictionary[key];
                }
                finally
                {
                    ReleaseReaderLock();
                }
            }
            set
            {
                Validate(key, value);

                AcquireWriterLock();
                try
                {
                    this._Dictionary[key] = value;
                }
                finally
                {
                    ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// 字典中是否包含某个键值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否包含某个键值</returns>
        public bool Contains(TKey key)
        {
            AcquireReaderLock();
            try
            {
                return this._Dictionary.ContainsKey(key);
            }
            finally
            {
                ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">数据</param>
        public void Add(TKey key, TValue data)
        {
            Validate(key, data);

            AcquireWriterLock();
            try
            {
                this._Dictionary.Add(key, data);
            }
            finally
            {
                ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="key"></param>
        public bool Remove(TKey key)
        {
            AcquireWriterLock();
            try
            {
                bool result = this._Dictionary.ContainsKey(key);

                if (result)
                    this._Dictionary.Remove(key);

                return result;
            }
            finally
            {
                ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 得到集合中的值，如果该值不存在，提供缺省值
        /// </summary>
        /// <typeparam name="T">缺省值的数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>集合中的值或缺省值</returns>
        public T GetValue<T>(TKey key, T defaultValue)
        {
            T result = defaultValue;

            object data = this[key];

            if (data != null)
                result = (T)DataConverter.ChangeType<object>(data, typeof(T));

            return result;
        }

        /// <summary>
        /// 字典项的个数
        /// </summary>
        public int Count
        {
            get
            {
                AcquireReaderLock();
                try
                {
                    return _Dictionary.Count;
                }
                finally
                {
                    ReleaseReaderLock();
                }
            }
        }

        protected void AcquireReaderLock()
        {
            RWLock.AcquireReaderLock(lockTimeout);
        }

        protected void AcquireWriterLock()
        {
            RWLock.AcquireWriterLock(lockTimeout);
        }

        protected void ReleaseReaderLock()
        {
            RWLock.ReleaseReaderLock();
        }

        protected void ReleaseWriterLock()
        {
            RWLock.ReleaseWriterLock();
        }

        /// <summary>
        /// 读写锁
        /// </summary>
        protected ReaderWriterLock RWLock
        {
            get
            {
                lock (this)
                {
                    if (this.rwLock == null)
                        this.rwLock = new ReaderWriterLock();

                    return this.rwLock;
                }
            }
        }

        private void Validate(TKey key, object value)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(key != null, "key");
        }

        #region 和Dictionary之间的交互
        /// <summary>
        /// Copy到Dictionary中
        /// </summary>
        /// <param name="dict"></param>
        public void CopyTo(IDictionary<TKey, object> dict)
        {
            this.CopyTo(dict, (kp) => true);
        }

        /// <summary>
        /// Copy到Dictionary中
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="predicate">带条件的复制</param>
        public void CopyTo(IDictionary<TKey, object> dict, Predicate<KeyValuePair<TKey, object>> predicate)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

            dict.Clear();

            AcquireReaderLock();
            try
            {
                foreach (KeyValuePair<TKey, object> kp in this)
                {
                    if (predicate(kp))
                        dict.Add(kp.Key, kp.Value);
                }
            }
            finally
            {
                ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 从Dictionary中复制
        /// </summary>
        /// <param name="dict"></param>
        public void CopyFrom(IDictionary<TKey, object> dict)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

            AcquireWriterLock();
            try
            {
                this._Dictionary.Clear();

                foreach (KeyValuePair<TKey, object> kp in dict)
                    this.Add(kp.Key, (TValue)kp.Value);
            }
            finally
            {
                ReleaseWriterLock();
            }
        }
        #endregion

        public IEnumerator GetEnumerator()
        {
            AcquireReaderLock();
            try
            {
                foreach (DictionaryEntry entry in this._Dictionary)
                    yield return new KeyValuePair<TKey, object>((TKey)entry.Key, entry.Value);
            }
            finally
            {
                ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 判断字典中是否有值非法的项
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static bool IsInvalidDictionary(Hashtable dictionary)
        {
            bool result = false;

            if (dictionary != null)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (entry.Value is UnknownSerializationType || entry.Value is TypeLoadException)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            AcquireReaderLock();
            try
            {
                foreach (DictionaryEntry entry in this._Dictionary)
                    yield return new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
            }
            finally
            {
                ReleaseReaderLock();
            }
        }

        #endregion

        #region IDictionary<TKey,TValue> Members


        public bool ContainsKey(TKey key)
        {
            return this.Contains(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                AcquireReaderLock();

                try
                {
                    List<TKey> result = new List<TKey>();

                    foreach (object key in this._Dictionary.Keys)
                        result.Add((TKey)key);

                    return result;
                }
                finally
                {
                    ReleaseReaderLock();
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                AcquireReaderLock();
                try
                {
                    List<TValue> result = new List<TValue>();

                    foreach (object data in this._Dictionary.Values)
                        result.Add((TValue)data);

                    return result;
                }
                finally
                {
                    ReleaseReaderLock();
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue outValue)
        {
            AcquireReaderLock();
            try
            {
                bool result = this._Dictionary.ContainsKey(key);
                outValue = default(TValue);

                if (result)
                    outValue = (TValue)this._Dictionary[key];

                return result;
            }
            finally
            {
                ReleaseReaderLock();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            AcquireWriterLock();

            try
            {
                this._Dictionary.Clear();
            }
            finally
            {
                ReleaseWriterLock();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.Contains(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int i = 0;

            foreach (KeyValuePair<TKey, TValue> kp in this)
                array[arrayIndex + i] = kp;
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        #endregion

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(DictionarySerializationKey, this._Dictionary);

            if (IsInvalidDictionary(this._Dictionary))
                info.AddValue("_SerializedDictionary", this._SerializedDictionary);
            else
                info.AddValue("_SerializedDictionary", SerializationHelper.SerializeObjectToString(this._Dictionary, SerializationFormatterType.Binary));
        }
    }

    /// <summary>
    /// 流程活动中的上下文
    /// </summary>
    [Serializable]
    public class WfActivityContext : WfContextDictionaryBase<string, object>
    {
        public WfActivityContext()
        {
        }

        public WfActivityContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// 流程中的上下文
    /// </summary>
    [Serializable]
    public class WfProcessContext : WfContextDictionaryBase<string, object>
    {
        public WfProcessContext()
        {
        }

        public WfProcessContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
