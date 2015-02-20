using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 样式中的Overflow的枚举值
	/// </summary>
	public enum HtmlStyleOverflowDefine
	{
		/// <summary>
		/// visible
		/// </summary>
		[EnumItemDescription("visible")]
		Visible,

		/// <summary>
		/// scroll
		/// </summary>
		[EnumItemDescription("scroll")]
		Scroll,

		/// <summary>
		/// hidden
		/// </summary>
		[EnumItemDescription("hidden")]
		Hidden,

		/// <summary>
		/// auto
		/// </summary>
		[EnumItemDescription("auto")]
		Auto
	}
}
