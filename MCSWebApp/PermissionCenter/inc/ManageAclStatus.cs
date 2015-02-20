using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	/// <summary>
	/// 表示授权管理的状态
	/// </summary>
	public enum ManageAclStatus
	{
		/// <summary>
		/// 表示未配置
		/// </summary>
		NoConfig,

		/// <summary>
		/// 表示不存在配置中指定的角色或应用与角色与配置不一致
		/// </summary>
		RoleNotExists,

		/// <summary>
		/// 表示配置的管理角色中无任何人员
		/// </summary>
		NobodyIn,

		/// <summary>
		/// 表示一切就绪
		/// </summary>
		Ready,
	}
}