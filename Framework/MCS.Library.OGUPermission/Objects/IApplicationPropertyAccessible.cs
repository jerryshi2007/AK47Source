using System;
namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示Application对象的属性访问功能
	/// </summary>
	public interface IApplicationPropertyAccessible : IPermissionPropertyAccessible
	{
		/// <summary>
		/// 获取权限的集合
		/// </summary>
		MCS.Library.OGUPermission.PermissionCollection Permissions { get; }
		/// <summary>
		/// 获取或设置资源级别
		/// </summary>
		string ResourceLevel { get; set; }
		/// <summary>
		/// 获取角色的集合
		/// </summary>
		MCS.Library.OGUPermission.RoleCollection Roles { get; }
	}
}
