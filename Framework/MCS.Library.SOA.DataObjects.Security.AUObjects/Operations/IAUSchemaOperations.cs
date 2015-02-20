using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Operations
{
	/// <summary>
	/// 定义管理单元架构的操作
	/// </summary>
	public interface IAUSchemaOperations
	{
		/// <summary>
		/// 添加一个管理架构定义
		/// </summary>
		/// <param name="schema">添加的管理架构定义</param>
		void AddAdminSchema(AUSchema schema);

		/// <summary>
		/// 删除一个管理架构的定义
		/// </summary>
		/// <param name="schema">删除的管理架构定义</param>
		void DeleteAdminSchema(AUSchema schema);
		/// <summary>
		/// 更新一个管理架构的定义
		/// </summary>
		/// <param name="schema">更新的管理架构定义</param>
		/// <remarks>（跟踪分类的变更和管理架构范围类别）</remarks>
		void UpdateAdminSchema(AUSchema schema);

		/// <summary>
		/// 添加一个管理架构定义的角色
		/// </summary>
		/// <param name="schema">管理架构定义</param>
		/// <param name="role">角色</param>
		/// <remarks>各管理单元应相应</remarks>
		void AddAdminSchemaRole(AUSchemaRole role, AUSchema schema);

		/// <summary>
		/// 删除一个管理架构定义的角色
		/// </summary>
		/// <param name="schema">管理架构定义</param>
		/// <param name="role">角色</param>
		/// <remarks></remarks>
		void DeleteAdminSchemaRole(AUSchemaRole role);

		/// <summary>
		/// 更新一个管理架构角色
		/// </summary>
		/// <param name="role"></param>
		/// <remarks>（跟踪CodeName的变更）</remarks>
		void UpdateAdminSchemaRole(AUSchemaRole role);
	}
}
