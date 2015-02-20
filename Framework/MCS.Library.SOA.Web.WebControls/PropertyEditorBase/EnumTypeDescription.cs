using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 枚举类型具体条目的描述对象，用于注册枚举类型
	/// </summary>
	[Serializable]
	public sealed class EnumItemPropertyDescription
	{
		public EnumItemPropertyDescription(ListItem listItem)
			: this(listItem.Value, listItem.Text)
		{
		}

		public EnumItemPropertyDescription(string value, string text)
		{
			this.Value = value;
			this.Text = text;
		}

		public EnumItemPropertyDescription()
		{

		}

		public string Value
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}
	}

	[Serializable]
	internal sealed class EnumTypePropertyDescription
	{
		public string EnumTypeName
		{
			get;
			set;
		}

		private List<EnumItemPropertyDescription> _Items = null;

		public List<EnumItemPropertyDescription> Items
		{
			get
			{
				if (this._Items == null)
					this._Items = new List<EnumItemPropertyDescription>();

				return this._Items;
			}
		}

		public static EnumTypePropertyDescription FromEnumTypeName(string enumTypeName)
		{
			enumTypeName.CheckStringIsNullOrEmpty("enumTypeName");

			Type type = Type.GetType(enumTypeName);

			EnumTypePropertyDescription result = null;

			if (type != null)
			{
				EnumItemDescriptionList itemDespList = EnumItemDescriptionAttribute.GetDescriptionList(type);

				result = new EnumTypePropertyDescription();

				result.EnumTypeName = enumTypeName;

				foreach (EnumItemDescription itemDesp in itemDespList)
				{
					EnumItemPropertyDescription eipd = new EnumItemPropertyDescription();

					eipd.Value = itemDesp.ShortName.IsNotEmpty() ? itemDesp.ShortName : itemDesp.EnumValue.ToString();
					eipd.Text = itemDesp.Description.IsNotEmpty() ? itemDesp.Description : itemDesp.Name;

					result.Items.Add(eipd);
				}
			}

			return result;
		}
	}

	public sealed class EnumItemPropertyDescriptionConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			EnumItemPropertyDescription desp = new EnumItemPropertyDescription();

			desp.Value = DictionaryHelper.GetValue(dictionary, "value", string.Empty);
			desp.Text = DictionaryHelper.GetValue(dictionary, "text", string.Empty);

			return desp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			EnumItemPropertyDescription desp = (EnumItemPropertyDescription)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.AddNonDefaultValue("value", desp.Value);
			dictionary.AddNonDefaultValue("text", desp.Text);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(EnumItemPropertyDescription) }; }
		}
	}

}
