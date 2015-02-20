using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// Ogu或授权对象的扩展方法
	/// </summary>
	public static class PermissionObjectExtensions
	{
		/// <summary>
		/// 转换为AppID的集合
		/// </summary>
		/// <param name="appRoles"></param>
		/// <returns></returns>
		public static List<string> ToApplicationIDs(this Dictionary<IApplication, RoleCollection> appRoles)
		{
			List<string> result = new List<string>();

			if (appRoles != null)
			{
				foreach (KeyValuePair<IApplication, RoleCollection> kp in appRoles)
					result.Add(kp.Key.ID);
			}

			return result;
		}

		/// <summary>
		/// 转换为AppID的集合
		/// </summary>
		/// <param name="appPermissions"></param>
		/// <returns></returns>
		public static List<string> ToApplicationIDs(this Dictionary<IApplication, PermissionCollection> appPermissions)
		{
			List<string> result = new List<string>();

			if (appPermissions != null)
			{
				foreach (KeyValuePair<IApplication, PermissionCollection> kp in appPermissions)
					result.Add(kp.Key.ID);
			}

			return result;
		}

		/// <summary>
		/// 转换为角色ID的集合
		/// </summary>
		/// <param name="appRoles"></param>
		/// <returns></returns>
		public static List<string> ToRoleIDs(this Dictionary<IApplication, RoleCollection> appRoles)
		{
			List<string> result = new List<string>();

			if (appRoles != null)
			{
				foreach (KeyValuePair<IApplication, RoleCollection> kp in appRoles)
					kp.Value.ForEach(role => result.Add(role.ID));
			}

			return result;
		}

		/// <summary>
		/// 转换为权限ID的集合
		/// </summary>
		/// <param name="appPermissions"></param>
		/// <returns></returns>
		public static List<string> ToPermissionIDs(this Dictionary<IApplication, PermissionCollection> appPermissions)
		{
			List<string> result = new List<string>();

			if (appPermissions != null)
			{
				foreach (KeyValuePair<IApplication, PermissionCollection> kp in appPermissions)
					kp.Value.ForEach(role => result.Add(role.ID));
			}

			return result;
		}
	}
}
