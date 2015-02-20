using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.WF.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.PropertyDefine
{
    [Serializable]
    [DataContract]
    public class ClientPropertyValue
    {
        private ClientPropertyDataType _DataType = ClientPropertyDataType.String;
        private string _AlterKey = null;
        private string _StringValue = null;

        public ClientPropertyValue()
        {
        }

        public ClientPropertyValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(Translator.Translate(Define.DefaultCulture, "键必须是非空白字符串", "key"));

            this._AlterKey = key;
        }

        public ClientPropertyValue(ClientPropertyDefine def)
        {
            InitFromPropertyDefine(def);
        }

        public ClientPropertyValue(ClientPropertyDefine def, string stringValue)
        {
            InitFromPropertyDefine(def);

            this._StringValue = InitDefaultValue(def.DataType, stringValue);
        }

        public ClientPropertyValue(ClientPropertyDefine def, ClientPropertyDataType dataType, string stringValue)
        {
            InitFromPropertyDefine(def);

            this._DataType = dataType;
            this._StringValue = InitDefaultValue(dataType, stringValue);
        }

        #region Properties

        /// <summary>
        /// 获取客户端属性的值
        /// </summary>
        public string Key
        {
            get
            {
                return _AlterKey;
            }
            set
            {
                this._AlterKey = value;
            }
        }

        /// <summary>
        /// 属性的字符串值
        /// </summary>
        public string StringValue
        {
            get
            {
                return this._StringValue;
            }

            set
            {
                this._StringValue = value;
            }
        }

        public ClientPropertyDataType DataType
        {
            get
            {
                return this._DataType;
            }
            set
            {
                this._DataType = value;
            }
        }
        #endregion Properties

        private void InitFromPropertyDefine(ClientPropertyDefine def)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(def != null, "def");

            this._AlterKey = def.Name;
            this._StringValue = def.DefaultValue;
            this._DataType = def.DataType;
            this._StringValue = InitDefaultValue(this._DataType, def.DefaultValue);
        }

        private static string InitDefaultValue(ClientPropertyDataType dataType, string defaultValue)
        {
            string result = defaultValue;

            if (defaultValue.IsNullOrEmpty() && dataType != ClientPropertyDataType.String)
            {
                object data = TypeCreator.GetTypeDefaultValue(dataType.ToRealType());

                if (data != null)
                    result = data.ToString();
            }

            return result;
        }
    }
}
