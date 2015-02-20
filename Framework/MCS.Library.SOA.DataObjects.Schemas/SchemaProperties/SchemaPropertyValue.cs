using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Validation;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	[Serializable]
	[XElementSerializable]
	[DebuggerDisplay("Value={StringValue}")]
	public class SchemaPropertyValue : IPropertyValueAccessor
	{
		private SchemaPropertyDefine _Definition = null;

		internal SchemaPropertyValue()
		{
		}

		public SchemaPropertyValue(SchemaPropertyDefine def)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(def != null, "def");

			this._Definition = def;
		}

		[XElementFieldSerialize(AlternateFieldName = "_SV")]
		private string _StringValue = null;

		/// <summary>
		/// 属性的字符串值
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

		public SchemaPropertyDefine Definition
		{
			get { return this._Definition; }
			set { this._Definition = value; }
		}

		[NonSerialized]
		private List<MCS.Library.Validation.Validator> _Validators = null;

		/// <summary>
		/// 返回所有校验器
		/// </summary>
		public IEnumerable<MCS.Library.Validation.Validator> Validators
		{
			get
			{
				if (this._Validators == null)
				{
					this._Validators = new List<MCS.Library.Validation.Validator>();

					this.Definition.Validators.ForEach(pvd => this._Validators.Add(pvd.GetValidator()));
				}

				return this._Validators;
			}
		}

		public SchemaPropertyValue Clone()
		{
			SchemaPropertyValue newValue = new SchemaPropertyValue(this._Definition);

			newValue._StringValue = this._StringValue;

			return newValue;
		}

		/// <summary>
		/// 将Property的Value，赋值到当前的Value中。仅仅赋值StringValue，不包括定义部分
		/// </summary>
		/// <param name="pv"></param>
		public void FromPropertyVaue(PropertyValue pv)
		{
			if (pv != null)
				this._StringValue = pv.StringValue;
		}

		public void SaveImageProperty()
		{
			if (this.StringValue.IsNotEmpty())
			{
				var img = JSONSerializerExecute.Deserialize<ImageProperty>(this.StringValue);
				if (img != null)
				{
					ImagePropertyAdapter.Instance.UpdateContent(img);
					this.StringValue = JSONSerializerExecute.Serialize(img);
				}
			}
		}

		public PropertyValue ToPropertyValue()
		{
			PropertyValue pv = new PropertyValue(this.Definition);

			pv.StringValue = this.StringValue;

			return pv;
		}

		public static implicit operator PropertyValue(SchemaPropertyValue spv)
		{
			spv.NullCheck("spv");

			return spv.ToPropertyValue();
		}

		public virtual void ToXElement(XElement element)
		{
			element.NullCheck("element");

			element.SetAttributeValue(this.Definition.Name, this.StringValue);
		}

		public virtual void FromXElement(XElement element)
		{
			element.NullCheck("element");

			this.StringValue = element.AttributeValue(this.Definition.Name);
		}

		PropertyDefine IPropertyValueAccessor.Definition
		{
			get { return this._Definition; }
			set { this._Definition = (SchemaPropertyDefine)value; }
		}
	}
}
