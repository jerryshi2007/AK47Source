using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
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
			if (string.IsNullOrEmpty(enumTypeName))
				throw new ArgumentNullException("enumTypeName");

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
}
