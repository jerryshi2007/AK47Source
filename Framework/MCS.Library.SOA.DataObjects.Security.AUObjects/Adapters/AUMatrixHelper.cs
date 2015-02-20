using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	public class AUMatrixHelper
	{
		/// <summary>
		/// 获取或设置一个值，表示是否启用角色定义的缓存
		/// </summary>
		public static bool DefinitionCacheEnabled { get; set; }

		/// <summary>
		/// 获取管理架构角色定义的角色矩阵维度定义
		/// </summary>
		/// <param name="schemaRoleID"></param>
		/// <returns></returns>
		public static SOARolePropertyDefinitionCollection GetSchemaRolePropertyDefinitions(string schemaRoleID)
		{
			var defines = DefinitionCacheEnabled ? SOARolePropertyDefinitionAdapter.Instance.GetByRoleID(schemaRoleID) : SOARolePropertyDefinitionAdapter.Instance.LoadByRoleID(schemaRoleID);
			return defines;
		}

		/// <summary>
		/// 载入管理单元的角色矩阵行集
		/// </summary>
		/// <param name="schemaRoleID">管理架构角色的ID</param>
		/// <param name="roleID">管理单元角色的ID</param>
		/// <returns></returns>
		public static SOARolePropertyRowCollection LoadUnitRolePropertyRows(string schemaRoleID, string roleID)
		{
			var defines = GetSchemaRolePropertyDefinitions(schemaRoleID);

			return SOARolePropertiesAdapter.Instance.LoadByRoleID(roleID, null, defines);
		}
		/// <summary>
		/// 在入管理架构的角色矩阵行集
		/// </summary>
		/// <param name="schemaRoleID"></param>
		/// <returns></returns>
		public static SOARolePropertyRowCollection LoadSchemaRolePropertyRows(string schemaRoleID)
		{
			return LoadUnitRolePropertyRows(schemaRoleID, schemaRoleID);
		}
	}
}
