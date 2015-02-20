using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	/// <summary>
	/// 表示AU操作的类型 
	/// </summary>
	public enum AUOperationType
	{
		/// <summary>
		/// 不执行任何操作
		/// </summary>
		[EnumItemDescription("不执行任何操作")]
		None,
		/// <summary>
		/// 添加管理架构
		/// </summary>
		[EnumItemDescription("添加管理架构")]
		AddAdminSchema,
		/// <summary>
		/// 移除管理架构
		/// </summary>
		[EnumItemDescription("删除管理架构")]
		RemoveAdminSchema,
		/// <summary>
		/// 更新管理架构
		/// </summary>
		[EnumItemDescription("更新管理架构")]
		UpdateAdminSchema,
		/// <summary>
		/// 添加管理单元
		/// </summary>
		[EnumItemDescription("添加管理单元")]
		AddAdminUnit,
		/// <summary>
		/// 移除管理单元
		/// </summary>
		[EnumItemDescription("移除管理单元")]
		RemoveAdminUnit,
		/// <summary>
		/// 更新管理单元
		/// </summary>
		[EnumItemDescription("更新管理单元")]
		UpdateAdminUnit,
		/// <summary>
		/// 添加管理单元角色
		/// </summary>
		[EnumItemDescription("添加管理架构角色")]
		AddSchemaRole,
		/// <summary>
		/// 移除管理单元角色
		/// </summary>
		[EnumItemDescription("移除管理架构角色")]
		RemoveSchemaRole,
		/// <summary>
		/// 更新管理架构角色
		/// </summary>
		[EnumItemDescription("更新管理架构角色")]
		UpdateAdminSchemaRole,
		/// <summary>
		/// 修改角色成员
		/// </summary>
		[EnumItemDescription("修改管理单元角色成员")]
		ModifyAURoleMembers,
		/// <summary>
		/// 添加管理范围项目
		/// </summary>
		[EnumItemDescription("添加管理单元管理范围对象")]
		AddAUScopeItem,
		/// <summary>
		/// 移除管理范围项目
		/// </summary>
		[EnumItemDescription("移除管理单元管理范围对象")]
		RemoveAUScopeItem,
		[EnumItemDescription("从角色中移除用户")]
		RemoveUserFromRole,
		[EnumItemDescription("移动管理单元")]
		MoveAdminUnit,
		[EnumItemDescription("将用户添加到角色")]
		AddUserToRole,
		[EnumItemDescription("更新管理范围的条件")]
		UpdateScopeCondition,
		[EnumItemDescription("更新对象访问控制列表")]
		UpdateObjectAcl,
		[EnumItemDescription("递归替换子对象访问控制列表")]
		ReplaceAclRecursively,
	}
}
