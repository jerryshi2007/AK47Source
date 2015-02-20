using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Operations
{
	/// <summary>
	/// 定义管理单元操作的接口
	/// </summary>
	public interface IAUOperations
	{
		/// <summary>
		/// 添加下级管理单元
		/// </summary>
		/// <param name="unit">一个<see cref="AdminUnit"/>，表示要添加的管理单元</param>
		/// <param name="parent">一个<see cref="AdminUnit"/>，表示要上级的管理单元 或为 <see langword="null"/>，表示顶级管理单元。</param>
		/// <exception cref="ArgumentNullException">参数<paramref name="unit"/>为<see langword="null" />。</exception>
		void AddAdminUnit(AdminUnit unit, AdminUnit parent);
		/// <summary>
		/// 导入模式添加下级管理单元
		/// </summary>
		/// <param name="unit">要添加的管理单元</param>
		/// <param name="parent">一个<see cref="AdminUnit"/>，表示要上级的管理单元 或为 <see langword="null"/>，表示顶级管理单元。</param>
		/// <param name="roles">此单元包含的角色</param>
		/// <param name="scopes">此单元包含的管理范围</param>
		void AddAdminUnitWithMembers(AdminUnit unit, AdminUnit parent, AURole[] roles, AUAdminScope[] scopes);

		/// <summary>
		/// 移动一个管理单元（含子级）
		/// </summary>
		/// <param name="unit">要移动的管理单元</param>
		/// <param name="newParent">将作为父级的<see cref="AdminUnit"/></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="unit"/>或<paramref name="newParent"/>为<see langword="null" />。</exception>
		void MoveAdminUnit(AdminUnit unit, AdminUnit newParent);

		/// <summary>
		/// 更新一个管理单元
		/// </summary>
		/// <param name="unit"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="unit"/>为<see langword="null" />。</exception>
		void UpdateAdminUnit(AdminUnit unit);

		/// <summary>
		/// 删除一个管理单元(对含子管理单元的无效)
		/// </summary>
		/// <param name="unit"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="unit"/>为<see langword="null" />。</exception>
		void DeleteAdminUnit(AdminUnit unit);

		/// <summary>
		/// 添加管理单元范围的固定成员
		/// </summary>
		/// <param name="item"></param>
		/// <param name="scope">添加到的范围</param>
		/// <exception cref="ArgumentNullException">参数<paramref name="item"/>或<paramref name="scope"/>为<see langword="null" />。</exception>
		void AddObjectToScope(AUAdminScopeItem item, AUAdminScope scope);
		/// <summary>
		/// 从管理单元的管理范围中移除一个固定成员
		/// </summary>
		/// <param name="item"></param>
		/// <param name="scope"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="item"/>或<paramref name="scope"/>为<see langword="null" />。</exception>
		void RemoveObjectFromScope(AUAdminScopeItem item, AUAdminScope scope);

		/// <summary>
		/// 更新管理范围的条件表达式
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="condition"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="scope"/>或<paramref name="condition"/>为<see langword="null" />。</exception>
		void UpdateScopeCondition(AUAdminScope scope, SCCondition condition);

		/// <summary>
		/// 向角色添加一个人员
		/// </summary>
		/// <param name="user"></param>
		/// <param name="role"></param>
		/// <param name="unit"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="user"/>或<paramref name="unit"/>或<paramref name="role"/>为<see langword="null" />。</exception>
		void AddUserToRole(PC.SCUser user, AdminUnit unit, AUSchemaRole role);

		/// <summary>
		/// 从角色移除一个人员
		/// </summary>
		/// <param name="user"></param>
		/// <param name="role"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="user"/>或<paramref name="unit"/>或<paramref name="role"/>为<see langword="null" />。</exception>
		void RemoveUserFromRole(PC.SCUser user, AdminUnit unit, AUSchemaRole role);
		/// <summary>
		/// 替换一个角色的用户
		/// </summary>
		/// <param name="users"></param>
		/// <param name="role"></param>
		/// <exception cref="ArgumentNullException">参数<paramref name="users"/>或<paramref name="unit"/>或<paramref name="role"/>为<see langword="null" />。</exception>
		void ReplaceUsersInRole(PC.SCUser[] users, AdminUnit unit, AUSchemaRole role);
	}
}
