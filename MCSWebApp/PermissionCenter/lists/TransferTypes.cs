using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	#region 枚举类型

	/// <summary>
	/// 进行复制或移动的对象类型
	/// </summary>
	[Flags]
	public enum TransferObjectType
	{
		None = 0,

		/// <summary>
		/// 包含用户
		/// </summary>
		Members = 1,

		/// <summary>
		/// 包含群组
		/// </summary>
		Groups = 2,

		/// <summary>
		/// 包含组织
		/// </summary>
		Orgnizations = 4,

		/// <summary>
		/// 包含根组织
		/// </summary>
		RootOrgnizations = 8,
	}

	/// <summary>
	/// 进行复制或移动的操作类型
	/// </summary>
	public enum TransferActionType
	{
		/// <summary>
		/// 无动作
		/// </summary>
		None,

		/// <summary>
		/// 人员复制到群组
		/// </summary>
		UserCopyToGroup,

		/// <summary>
		/// 人员复制到组织
		/// </summary>
		UserCopyToOrg,

		/// <summary>
		/// 人员移动到组织
		/// </summary>
		UserMoveToOrg,

		/// <summary>
		/// 群组移动到组织
		/// </summary>
		GroupMoveToOrg,

		/// <summary>
		/// 组织转移
		/// </summary>
		OrgTransfer,

		/// <summary>
		/// 移动到组织(混合)
		/// </summary>
		MixedToOrg,
	}
	#endregion
}