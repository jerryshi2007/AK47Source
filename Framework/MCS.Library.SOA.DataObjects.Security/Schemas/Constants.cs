using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示标准对象模式的类型
	/// </summary>
	public enum StandardObjectSchemaType
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

	/// <summary>
	/// 表示用户集合中按照状态筛选的枚举
	/// </summary>
	public enum SchemaObjectStatusFilterTypes
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,

		/// <summary>
		/// 正常
		/// </summary>
		Normal = 1,

		/// <summary>
		/// 已删除
		/// </summary>
		Deleted = 2,

		/// <summary>
		/// 全部
		/// </summary>
		All = 3
	}

	/// <summary>
	/// 表示查询快照时的查询ID的类型
	/// </summary>
	public enum SnapshotQueryIDType
	{
		/// <summary>
		/// 按对象ID
		/// </summary>
		[EnumItemDescription("对象的ID", ShortName = "ID")]
		Guid,

		/// <summary>
		/// 按对象的代码名称
		/// </summary>
		[EnumItemDescription("Code", ShortName = "CodeName")]
		CodeName
	}

	/// <summary>
	/// 内置函数使用的对象查询的ID类型
	/// </summary>
	public enum BuiltInFunctionIDType
	{
		/// <summary>
		/// 按对象ID
		/// </summary>
		[EnumItemDescription("对象的ID", ShortName = "ID")]
		Guid,

		/// <summary>
		/// 按对象的代码名称
		/// </summary>
		[EnumItemDescription("代码名称", ShortName = "CodeName")]
		CodeName,

		/// <summary>
		/// 按对象的全路径
		/// </summary>
		[EnumItemDescription("全路径", ShortName = "FullPath")]
		FullPath
	}
}
