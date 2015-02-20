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
    /// Cache���֪ͨ����
    /// </summary>
    public enum CacheNotifyType
    {
        /// <summary>
        /// �����Ѿ��Ƿ�
        /// </summary>
        Invalid = 1,

        /// <summary>
        /// �����޸ģ����֪ͨ�У������Ѿ����������
        /// </summary>
        Update = 2,

        /// <summary>
        /// �����������
        /// </summary>
        Clear = 3
    }

    /// <summary>
    /// Cache���ʱ�����ݰ�
    /// </summary>
    [Serializable]
    public class CacheNotifyData
    {
        /// <summary>
        /// �ռ�������
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
        /// ���췽��
        /// </summary>
        public CacheNotifyData()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="queueType">Cache���е�����</param>
        /// <param name="key">Cache��key</param>
        /// <param name="nType">֪ͨ������</param>
        public CacheNotifyData(Type queueType, object key, CacheNotifyType nType)
        {
            InitTypeDesp(queueType);
            this.cacheKey = key;
            this.notifyType = nType;
        }

        /// <summary>
        /// ��Stream��ԭCacheNotifyData
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
        /// ��byte[]��ԭCacheNotifyData
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
        /// ֪ͨ������
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
        /// Cache���е�����
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
        /// Cache���е���������
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
        /// Cache�������
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
        /// Cache���Key
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
        /// ת��Ϊ�ֽ�����
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
            ExceptionHelper.FalseThrow(queueType.IsSubclassOf(typeof(CacheQueueBase)), "queueType���ͱ�����CacheQueueBase��������");

            this.cacheQueueTypeDesp = queueType.AssemblyQualifiedName;
            this.cacheQueueType = queueType;
        }
    }
}
