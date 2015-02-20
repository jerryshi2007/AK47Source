using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.Design;
using System.Security.Permissions;
using System.ComponentModel;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 表示一般占位用控件的设计器
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class GenericControlDesigner : ControlDesigner
	{
		/// <summary>
		/// 检索用于在设计时表示控件的 HTML 标记。
		/// </summary>
		/// <returns>用于在设计时表示控件的 HTML 标记。</returns>
		public override string GetDesignTimeHtml()
		{
			return this.CreatePlaceHolderDesignTimeHtml();
		}
	}
}
