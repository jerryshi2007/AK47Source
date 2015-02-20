using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Cache变更通知类型
    /// </summary>
    public enum CacheNotifyType
    {
        /// <summary>
        /// 数据已经非法
        /// </summary>
        Invalid = 1,

        /// <summary>
        /// 数据修改，变更通知中，包含已经变更的数据
        /// </summary>
        Update = 2,

        /// <summary>
        /// 清除所有数据
        /// </summary>
        Clear = 3
    }

    /// <summary>
    /// Cache变更时的数据包
    /// </summary>
    [Serializable]
    public class CacheNotifyData
    {
        /// <summary>
        /// 空集合数组
        /// </summary>
        public static readonly CacheNotifyData[] EmptyData = new CacheNotifyData[0];

        private const string VersionMatchTemplate = @"Version=([0-9.]{1,})(,)";

        private CacheNotifyType notifyType = CacheNotifyType.Invalid;

        private string cacheQueueTypeDesp = string.Empty;

        private object cacheKey = null;
        private object cacheData = null;

        [NonSerialized]
        private Type cacheQueueType = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public CacheNotifyData()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="queueType">Cache队列的类型</param>
        /// <param name="key">Cache的key</param>
        /// <param name="nType">通知的类型</param>
        public CacheNotifyData(Type queueType, object key, CacheNotifyType nType)
        {
            InitTypeDesp(queueType);
            this.cacheKey = key;
            this.notifyType = nType;
        }

        /// <summary>
        /// 从Stream还原CacheNotifyData
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static CacheNotifyData FromBuffer(Stream stream)
        {
            stream.NullCheck("stream");

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Binder = VersionStrategyBinder.Instance;

            return (CacheNotifyData)formatter.Deserialize(stream);
        }

        /// <summary>
        /// 从byte[]还原CacheNotifyData
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static CacheNotifyData FromBuffer(byte[] buffer)
        {
            buffer.NullCheck("buffer");

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return FromBuffer(stream);
            }
        }

        /// <summary>
        /// 通知的类型
        /// </summary>
        public CacheNotifyType NotifyType
        {
            get
            {
                return this.notifyType;
            }
            set
            {
                this.notifyType = value;
            }
        }

        /// <summary>
        /// Cache队列的类型
        /// </summary>
        public Type CacheQueueType
        {
            get
            {
                if (this.cacheQueueType == null && string.IsNullOrEmpty(this.cacheQueueTypeDesp) == false)
                {
                    string typeName = Regex.Replace(this.cacheQueueTypeDesp, VersionMatchTemplate, string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    TypeCreator.TryGetTypeInfo(typeName, out this.cacheQueueType);
                }

                return this.cacheQueueType;
            }
        }

        /// <summary>
        /// Cache队列的类型描述
        /// </summary>
        public string CacheQueueTypeDesp
        {
            get
            {
                return this.cacheQueueTypeDesp;
            }
            set
            {
                this.cacheQueueTypeDesp = value;
                this.cacheQueueType = null;
            }
        }

        /// <summary>
        /// Cache项的数据
        /// </summary>
        public object CacheData
        {
            get
            {
                return this.cacheData;
            }
            set
            {
                this.cacheData = value;
            }
        }

        /// <summary>
        /// Cache项的Key
        /// </summary>
        public object CacheKey
        {
            get
            {
                return this.cacheKey;
            }
            set
            {
                this.cacheKey = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Format("NotifyType: {0}, CacheQueue: {1}, Cache Key: {2}, Cache Data: {3}",
                    this.NotifyType,
                    this.CacheQueueTypeDesp,
                    this.CacheKey,
                    this.CacheData
                );

            return result;
        }

        /// <summary>
        /// 转换为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(ms, this);

                return ms.ToArray();
            }
        }

        private void InitTypeDesp(Type queueType)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(queueType != null, "queueType");
            ExceptionHelper.FalseThrow(queueType.IsSubclassOf(typeof(CacheQueueBase)), "queueType类型必须是CacheQueueBase的派生类");

            this.cacheQueueTypeDesp = queueType.AssemblyQualifiedName;
            this.cacheQueueType = queueType;
        }
    }
}
