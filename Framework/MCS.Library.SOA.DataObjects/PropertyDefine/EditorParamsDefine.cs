using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class EditorParamsDefine : Dictionary<string, string>
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public string EnumTypeName
        {
            get
            {
                if (this.ContainsKey("enumTypeName") == true)
                    return this["enumTypeName"];
                else
                    return string.Empty;
            }
            set
            {
                this["enumTypeName"] = value;
            }
        }

        /// <summary>
        /// 枚举类型
        /// </summary>
        public string CloneControlID
        {
            get
            {
                if (this.ContainsKey("cloneControlID") == true)
                    return this["cloneControlID"];
                else
                    return string.Empty;
            }
            set
            {
                this["cloneControlID"] = value;
            }
        }

        internal List<ControlPropertyDefine> _ServerControlProperties = null;
        /// <summary>
        /// 服务端控件属性定义
        /// </summary>
        public List<ControlPropertyDefine> ServerControlProperties
        {
            get
            {
                if (this._ServerControlProperties == null)
                    this._ServerControlProperties = new List<ControlPropertyDefine>();

                return this._ServerControlProperties;
            }
        }

        public bool UseTemplate
        {
            get
            {
                if (this.ContainsKey("useTemplate"))
                    return bool.Parse(this["useTemplate"]);
                else
                    return false;
            }
        }

        /// <summary>
        /// 扩展设置
        /// </summary>
        public string ExtendedSettings
        {
            get
            {
                if (this.ContainsKey("extendedSettings") == true)
                    return this["extendedSettings"];
                else
                    return string.Empty;
            }
            set
            {
                this["extendedSettings"] = value;
            }
        }
    }

    public sealed class ControlPropertyDefine
    {
        public string PropertyName { get; set; }

        public string StringValue { get; set; }

        private PropertyDataType _DataType = PropertyDataType.String;
        public PropertyDataType DataType
        {
            get { return this._DataType; }
            set { this._DataType = value; }
        }

        /// <summary>
        /// 得到强类型的值
        /// </summary>
        /// <returns></returns>
        public object GetRealValue()
        {
            object result = StringValue;

            Type realType = typeof(string);

            if (this.DataType.TryToRealType(out realType))
                result = DataConverter.ChangeType(result, realType);

            return result;
        }
    }

    public sealed class EditorParamsDefineConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            EditorParamsDefine paDefine = new EditorParamsDefine();

            foreach (KeyValuePair<string, object> item in dictionary)
            {
                if (string.Compare(item.Key, "serverControlProperties", true) == 0)
                {
                    string strServerControlProperties = DictionaryHelper.GetValue(dictionary, "serverControlProperties", string.Empty);
                    if (strServerControlProperties.IsNotEmpty())
                    {
                        List<ControlPropertyDefine> desControlProperties = JSONSerializerExecute.Deserialize<List<ControlPropertyDefine>>(strServerControlProperties);
                        desControlProperties.ForEach(pcd =>
                        {
                            paDefine.ServerControlProperties.Add(pcd);
                        });
                    }
                }
                else
                {
                    paDefine.Add(item.Key, item.Value != null ? item.Value.ToString() : string.Empty);
                }
            }

            return paDefine;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            EditorParamsDefine paDefine = (EditorParamsDefine)obj;

            foreach (KeyValuePair<string, string> item in paDefine)
            {
                if (string.IsNullOrEmpty(item.Value) == false)
                    dictionary.Add(item.Key, item.Value);
            }

            if (paDefine.ServerControlProperties.Count > 0)
                dictionary.Add("serverControlProperties", JSONSerializerExecute.Serialize(paDefine.ServerControlProperties));

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(EditorParamsDefine) }; }
        }
    }

    public sealed class ControlPropertyDefineConverter : JavaScriptConverter
    {

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            ControlPropertyDefine controlDefine = new ControlPropertyDefine();
            controlDefine.DataType = DictionaryHelper.GetValue(dictionary, "dataType", PropertyDataType.String);
            controlDefine.StringValue = DictionaryHelper.GetValue(dictionary, "stringValue", string.Empty);
            controlDefine.PropertyName = DictionaryHelper.GetValue(dictionary, "propertyName", string.Empty);
            return controlDefine;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            ControlPropertyDefine controlDefine = (ControlPropertyDefine)obj;
            dictionary.Add("propertyName", controlDefine.PropertyName);
            dictionary.Add("stringValue", controlDefine.StringValue);

            if (controlDefine.DataType != PropertyDataType.String)
                dictionary.Add("dataType", controlDefine.DataType);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new System.Type[] { typeof(ControlPropertyDefine) }; }
        }
    }
}
