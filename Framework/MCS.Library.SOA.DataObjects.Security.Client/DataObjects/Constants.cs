using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 表示标准对象模式的类型
	/// </summary>
	public enum ClientStandardObjectSchemaType
	{
		/// <summary>
		/// 组织
		/// </summary>
		Organizations = 1,

		/// <summary>
		/// 人员
		/// </summary>
		Users = 2,

		/// <summary>
		/// 群组
		/// </summary>
		Groups = 4,

		/// <summary>
		/// 关系对象
		/// </summary>
		RelationObjects = 8,

		/// <summary>
		/// 应用程序
		/// </summary>
		Applications = 16,

		/// <summary>
		/// 角色
		/// </summary>
		Roles = 32,

		/// <summary>
		/// 功能
		/// </summary>
		Permissions = 64,

		/// <summary>
		/// 成员关系
		/// </summary>
		MemberRelations = 128,

		/// <summary>
		/// 秘书关系
		/// </summary>
		SecretaryRelations = 256
	}
}
