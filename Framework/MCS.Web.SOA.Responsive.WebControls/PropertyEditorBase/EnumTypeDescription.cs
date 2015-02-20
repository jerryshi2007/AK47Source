using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
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
}
