using MCS.Library.Core;
using MCS.Library.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Common;

namespace MCS.Library.WF.Contracts.PropertyDefine
{
    [Serializable]
    [DataContract]
    public class ClientPropertyValueCollection : SerializableEditableKeyedDataObjectCollectionBase<string, ClientPropertyValue>
    {
        /// <summary>
        /// 初始化<see cref="ClientPropertyValueCollection"/>的新实例
        /// </summary>
        public ClientPropertyValueCollection()
        {
        }

        /// <summary>
        /// 使用指定的<see cref="SerializationInfo"/>和<see cref="StreamingContext"/>初始化<see cref="ClientPropertyValueCollection"/>的新实例
        /// </summary>
        /// <param name="info">存储将对象序列化或反序列化所需的全部数据</param>
        /// <param name="context">序列化描述的上下文</param>
        protected ClientPropertyValueCollection(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        protected override string GetKeyForItem(ClientPropertyValue item)
        {
            return item.Key;
        }

        /// <summary>
        /// 获取集合中指定键所对应的值
        /// </summary>
        /// <typeparam name="T">默认值的类型</typeparam>
        /// <param name="defaultValue">缺省值</param>
        /// <returns> 属性的值 或 缺省值</returns>
        public T GetValue<T>(ClientPropertyDefine def, T defaultValue)
        {
            T result = defaultValue;

            def.NullCheck("def");

            ClientPropertyValue v = this[def.Name];

            if (v != null)
            {
                result = (T)DataConverter.ChangeType(v.StringValue, result.GetType());
            }

            return result;
        }

        /// <summary>
        /// 获取属性的值，如果不存在该属性，则返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            ClientPropertyValue v = this[key];

            if (v != null)
                return (T)DataConverter.ChangeType(v.StringValue, typeof(T));
            else
                return defaultValue;
        }

        /// <summary>
        /// 设置属性的值。如果该属性不存在，则抛出异常
        /// </summary>
        /// <param name="def">属性定义</param>
        /// <param name="name"></param>
        /// <param name="stringValue"></param>
        public void SetValue(ClientPropertyDefine def, string stringValue)
        {
            this.TrySetValue(def, stringValue).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能找到名称为{0}的属性", def.Name));
        }

        public void SetValue(string key, string stringValue)
        {
            this.TrySetValue(key, stringValue).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能找到名称为{0}的属性", key));
        }

        /// <summary>
        /// 尝试去设置属性。如果该属性不存在，则返回false，否则返回true
        /// </summary>
        /// <param name="def"></param>
        /// <param name="stringValue">空值，将取默认值</param>
        /// <returns></returns>
        public bool TrySetValue(ClientPropertyDefine def, string stringValue)
        {
            return TrySetValue(def.Name, stringValue);
        }

        /// <summary>
        /// 在没有定义的情况下，设置属性的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dataType">类型</param>
        /// <param name="stringValue">属性值</param>
        public void AddOrSetValue<T>(string key, ClientPropertyDataType dataType, T value)
        {
            ClientPropertyValue v = this[key];
            string stringValue = string.Empty;

            if (value != null)
                stringValue = value.ToString();

            if (v == null)
            {
                v = new ClientPropertyValue(key) { DataType = dataType, StringValue = stringValue };
                this.AddNotExistsItem(v);
            }
            else if (v.DataType != dataType)
            {
                throw new ArgumentException(Translator.Translate(Define.DefaultCulture, "已存在键为{0}的属性，但类型为{1}，与预期的类型{2}不一致。", key, v.DataType, dataType));
            }

            v.StringValue = stringValue;
        }

        public void AddOrSetValue<T>(string key, T value)
        {
            ClientPropertyDataType dataType = typeof(T).ToClientPropertyDataType();

            this.AddOrSetValue(key, dataType, value);
        }

        /// <summary>
        ///  尝试去设置属性。如果该属性不存在，则返回false，否则返回true
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        private bool TrySetValue(string key, string stringValue)
        {
            ClientPropertyValue v = this[key];

            bool result = v != null;

            if (result)
            {
                if (stringValue != null)
                    v.StringValue = stringValue;
                else
                    v.StringValue = string.Empty;
            }

            return result;
        }
    }
}
