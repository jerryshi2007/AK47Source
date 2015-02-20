using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Validation;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [XElementSerializable]
    public class PropertyValue : IXElementSerializable, IPropertyValueAccessor, ISimpleXmlSerializer
    {
        internal PropertyValue()
        {
        }

        public PropertyValue(PropertyDefine def)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(def != null, "def");

            this._Definition = def;
        }

        [XElementFieldSerialize(AlternateFieldName = "_SV")]
        private string _StringValue = null;

        /// <summary>
        /// 属性的字符串值。如果StringValue为空，则使用默认值
        /// </summary>
        public string StringValue
        {
            get
            {
				string result = this._StringValue;

				if (result == null)
				{
					result = this.Definition.DefaultValue;

					if (this.Definition.DefaultValue.IsNullOrEmpty() && this.Definition.DataType != PropertyDataType.String)
					{
						object data = TypeCreator.GetTypeDefaultValue(this.Definition.DataType.ToRealType());

						if (data != null)
							result = data.ToString();
					}
				}

				return result;
            }
            set
            {
                this._StringValue = value;
            }
        }

        /// <summary>
        /// 得到强类型的值
        /// </summary>
        /// <returns></returns>
        public object GetRealValue()
        {
            object result = StringValue;

            Type realType = typeof(string);

            if (this.Definition.DataType.TryToRealType(out realType))
                result = DataConverter.ChangeType(result, realType);

            return result;
        }

        [XElementFieldSerialize(AlternateFieldName = "_Def")]
        private PropertyDefine _Definition = null;

        public PropertyDefine Definition
        {
            get { return this._Definition; }
            set { this._Definition = value; }
        }

        [NonSerialized]
        private List<Validator> _Validators = null;

        /// <summary>
        /// 返回所有校验器
        /// </summary>
        public IEnumerable<Validator> Validators
        {
            get
            {
                if (this._Validators == null)
                {
                    this._Validators = new List<Validator>();

                    this.Definition.Validators.ForEach(pvd => this._Validators.Add(pvd.GetValidator()));
                }

                return this._Validators;
            }
        }

        /// <summary>
        /// 校验属性值
        /// </summary>
        /// <param name="results"></param>
        public void Validate(ValidationResults results)
        {
            results.NullCheck("results");

            if (((List<Validator>)this.Validators).Count > 0)
            {
                object data = GetRealValue();

                this.Validators.ForEach(v => v.Validate(data, results));
            }
        }

        /// <summary>
        /// 复制属性的值
        /// </summary>
        /// <returns></returns>
        public PropertyValue Clone()
        {
            PropertyValue newValue = new PropertyValue(this._Definition);

            newValue._StringValue = this._StringValue;

            return newValue;
        }

        /// <summary>
        /// 是不是缺省值
        /// </summary>
        /// <returns></returns>
        public bool IsDefaultValue()
        {
            string sv = this._StringValue;

            if (this._StringValue.IsNullOrEmpty())
                sv = string.Empty;

            string dv = this._Definition.DefaultValue;

            if (this._Definition.DefaultValue.IsNullOrEmpty())
                dv = string.Empty;

            return sv == dv || (sv.IsNullOrEmpty() && dv.IsNotEmpty());
        }

        public void Serialize(XElement node, XmlSerializeContext context)
        {
            PropertyValue pv = this.Clone();
            int objID = 0;
            if (pv.Definition.DefaultValue.IsNullOrEmpty())
                pv.Definition.DefaultValue = string.Empty;

            if (pv.StringValue.IsNullOrEmpty())
                pv.StringValue = string.Empty;

            if (context.ObjectContext.TryGetValue(pv, out objID) == false)
            {
                objID = context.CurrentID;
                context.ObjectContext.Add(pv, objID);
                node.SetAttributeValue("id", context.CurrentID++);
                pv.Definition.Serialize(node, context);
            }
            else
            {
                node.SetAttributeValue("v", objID);
            }

            if (!pv.StringValue.IsNullOrEmpty() && pv.Definition.DefaultValue.ToLower() != pv.StringValue.ToLower())
                node.SetAttributeValue("_sv", pv.StringValue);

        }

        public void Deserialize(XElement node, XmlDeserializeContext context)
        {
            object pv = null;

            int objID = node.Attribute("v", -1);

            if (context.ObjectContext.TryGetValue(objID, out pv) == true)
            {
                this._Definition = ((PropertyValue)pv).Clone().Definition;
            }
            else
            {
                this._Definition = new PropertyDefine();
                this._Definition.Deserialize(node, context);

                objID = node.Attribute("id", -1);
                if (objID > -1)
                {
                    context.ObjectContext.Add(objID, this);
                }
            }

            string sv = node.Attribute("_sv", this._StringValue);

            if (string.IsNullOrEmpty(sv))
                this._StringValue = this._Definition.DefaultValue;
            else
                this._StringValue = sv;
        }

        /// <summary>
        /// 为通用的序列化而显示实现接口IXElementSerializable
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        void IXElementSerializable.Serialize(XElement node, XmlSerializeContext context)
        {
            if (this.Definition.DefaultValue.IsNullOrEmpty())
                this.Definition.DefaultValue = string.Empty;

            if (this.StringValue.IsNullOrEmpty())
                this.StringValue = string.Empty;

            this.Definition.Serialize(node, context);

            if (!this.StringValue.IsNullOrEmpty() && this.Definition.DefaultValue.ToLower() != this.StringValue.ToLower())
                node.SetAttributeValue("_sv", this.StringValue);
        }

        /// <summary>
        /// 为通用的序列化而显示实现接口IXElementSerializable
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        void IXElementSerializable.Deserialize(XElement node, XmlDeserializeContext context)
        {
            this._Definition = new PropertyDefine();
            this._Definition.Deserialize(node, context);

            string sv = node.Attribute("_sv", this._StringValue);

            if (string.IsNullOrEmpty(sv))
                this._StringValue = this._Definition.DefaultValue;
            else
                this._StringValue = sv;
        }

        public override int GetHashCode()
        {
			//沈峥注释，改为调用系统默认的方法，避免出现重复的Hash
            //return GetStringForHashCode().GetHashCode();
			return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PropertyValue pv = obj as PropertyValue;

            return this.GetStringForHashCode() == pv.GetStringForHashCode();
        }

        private string GetStringForHashCode()
        {
            string strFormat = "{0}@{1}@";
            StringBuilder strB = new StringBuilder(100);
            #region "原徐磊"
            strB.AppendFormat(strFormat, _Definition.Name, _Definition.EditorKey);
            strB.AppendFormat(strFormat, _Definition.DataType, _Definition.DefaultValue);
            strB.AppendFormat(strFormat, _Definition.DisplayName, _Definition.Description);
            strB.AppendFormat(strFormat, _Definition.ReadOnly, _Definition.Visible);
            #endregion

            return strB.ToString();
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (this.StringValue.IsNotEmpty())
            {
                string attrName = this.Definition.Name.Trim(' ', '\t');
                element.SetAttributeValue(attrName, this.StringValue);
            }
        }

        #endregion
    }
}
