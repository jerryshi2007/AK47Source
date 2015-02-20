using System;
namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 表示IUser对象的属性访问器
	/// </summary>
	public interface IUserPropertyAccessible : IOguPropertyAccessible
	{
		/// <summary>
		/// 获取一个集合，表示用户的相关兼职信息
		/// </summary>
		MCS.Library.OGUPermission.OguObjectCollection<MCS.Library.OGUPermission.IUser> AllRelativeUserInfo { get; }
		/// <summary>
		/// 获取或设置用户的干部属性。
		/// </summary>
		MCS.Library.OGUPermission.UserAttributesType Attributes { get; set; }
		/// <summary>
		/// 获取或设置Email
		/// </summary>
		string Email { get; set; }
		/// <summary>
		/// 获取或设置一个值，表示此用户是否为兼职
		/// </summary>
		bool IsSideline { get; set; }
		/// <summary>
		/// 获取或设置用户的登录名
		/// </summary>
		string LogOnName { get; set; }
		/// <summary>
		/// 获取一个集合，表示用户的群组
		/// </summary>
		MCS.Library.OGUPermission.OguObjectCollection<MCS.Library.OGUPermission.IGroup> MemberOf { get; }

		/// <summary>
		/// 获取或设置表示用户的职位的字符串
		/// </summary>
		string Occupation { get; set; }

		/// <summary>
		/// 获取一个集合，表示用户所拥有的功能
		/// </summary>
		MCS.Library.OGUPermission.UserPermissionCollection Permissions { get; }

		/// <summary>
		/// 获取或设置用户的级别
		/// </summary>
		MCS.Library.OGUPermission.UserRankType Rank { get; set; }
		/// <summary>
		/// 获取一个集合，表示用户拥有的角色
		/// </summary>
		MCS.Library.OGUPermission.UserRoleCollection Roles { get; }
		/// <summary>
		/// 获取一个集合，表示用户的秘书
		/// </summary>
		MCS.Library.OGUPermission.OguObjectCollection<MCS.Library.OGUPermission.IUser> Secretaries { get; }
		/// <summary>
		/// 获取一个集合，表示此用户是谁的秘书
		/// </summary>
		MCS.Library.OGUPermission.OguObjectCollection<MCS.Library.OGUPermission.IUser> SecretaryOf { get; }
	}
}
