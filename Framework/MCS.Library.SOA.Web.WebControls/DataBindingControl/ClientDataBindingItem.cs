using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class ClientDataBindingItem : DataBindingItem
	{
		public ClientDataBindingItem()
		{
		}

		public ClientDataBindingItem(DataBindingItem item)
		{
			CopyFromBindingItem(item);
		}

		public void CopyFromBindingItem(DataBindingItem item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			this.ControlID = item.ControlID;
			this.ControlPropertyName = item.ControlPropertyName;
			this.ClientIsHtmlElement = item.ClientIsHtmlElement;
			this.ClientDataPropertyName = item.ClientDataPropertyName;
			this.ClientPropName = item.ClientPropName;
			this.ClientSetPropName = item.ClientSetPropName;
			this.ClientDataType = item.ClientDataType;
			this.IsValidate = item.IsValidate;
			this.IsValidateOnBlur = item.IsValidateOnBlur;
			this.DataPropertyName = item.DataPropertyName;
			this.Direction = item.Direction;
			this.Format = item.Format;
			this.EnumAutoBinding = item.EnumAutoBinding;
			this.ItemTrimBlankType = item.ItemTrimBlankType;
			this.EnumUsage = item.EnumUsage;
			this.ValidationGroup = item.ValidationGroup;
			this.AutoFormatOnBlur = item.AutoFormatOnBlur;
		}
	}
}
