using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public enum WfClientVariableDataType
    {
        /// <summary>
        /// DataTypeDescription
        /// </summary>
        String,

        /// <summary>
        /// 整型
        /// </summary>
        Int,

        /// <summary>
        /// 双精度浮点型
        /// </summary>
        Double,

        /// <summary>
        /// 单精度浮点型
        /// </summary>
        Float,

        /// <summary>
        /// 布尔型
        /// </summary>
        Boolean,

        /// <summary>
        /// 日期型
        /// </summary>
        DateTime,

        /// <summary>
        /// 字符串数组
        /// </summary>
        StringArray,

        /// <summary>
        /// 整型数组
        /// </summary>
        IntArray,

        /// <summary>
        /// 布尔型数组
        /// </summary>
        BooleanArray
    }

    [DataContract]
    [Serializable]
    public class WfClientVariableDescriptor : WfClientKeyedDescriptorBase
    {
        private WfClientVariableDataType _OriginalType = WfClientVariableDataType.String;
        private string _OriginalValue = string.Empty;

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public WfClientVariableDescriptor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public WfClientVariableDescriptor(string key)
            : base(key)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalValue"></param>
        public WfClientVariableDescriptor(string key, string originalValue)
            : this(key, originalValue, WfClientVariableDataType.String)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dataType"></param>
        public WfClientVariableDescriptor(string key, string originalValue, WfClientVariableDataType dataType)
            : this(key)
        {
            this._OriginalType = dataType;
            this._OriginalValue = originalValue;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public WfClientVariableDataType OriginalType
        {
            get
            {
                return this._OriginalType;
            }
            set
            {
                this._OriginalType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OriginalValue
        {
            get
            {
                return this._OriginalValue;
            }
            set
            {
                this._OriginalValue = value;
            }
        }
    }

    [Serializable]
    [DataContract]
    public class WfClientVariableDescriptorCollection : WfClientKeyedDescriptorCollectionBase<WfClientVariableDescriptor>
    {
        public WfClientVariableDescriptorCollection()
        {
        }

        protected WfClientVariableDescriptorCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// 设置变量的值，如果不存在，则增加一个变量，否则替换掉
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalValue"></param>
        public void AddOrSetValue(string key, string originalValue)
        {
            this.AddOrSetValue(key, originalValue, WfClientVariableDataType.String);
        }

        /// <summary>
        /// 设置变量的值，如果不存在，则增加一个变量，否则替换掉
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalValue"></param>
        /// <param name="dataType"></param>
        public void AddOrSetValue(string key, string originalValue, WfClientVariableDataType dataType)
        {
            if (this.ContainsKey(key))
                this.Remove(v => v.Key == key);

            this.Add(new WfClientVariableDescriptor(key, originalValue, dataType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            T result = defaultValue;

            WfClientVariableDescriptor data = this[key];

            if (data != null)
                result = (T)DataConverter.ChangeType<object>(data.OriginalValue, typeof(T));

            return result;
        }
    }
}
