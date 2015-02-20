using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Applications.AppAdmin.Common
{
	/// <summary>
	/// 应用的几个继承对象的掩码值
	/// </summary>
	[Flags]
	public enum InheritedState
	{
		/// <summary>
		///没有继承 
		/// </summary>
		NONE = 0,
		/// <summary>
		/// 服务范围的掩码，1
		/// </summary>
		SCOPES = 1,
		/// <summary>
		/// 角色的掩码，2
		/// </summary>
		ROLES = 2,
		/// <summary>
		/// 功能的掩码，4
		/// </summary>
		FUNCTIONS = 4,
		/// <summary>
		/// 角色功能关系的掩码，8
		/// </summary>
		ROLE_TO_FUNCTIONS = 8,
		/// <summary>
		/// 被授权对象的掩码，16
		/// </summary>
		OBJECT = 16
	}
}
