using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public static class AUPrincipalExtension
	{
		/// <summary>
		/// 是否是超级管理员
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public static bool IsSupervisor(IPrincipal principal)
		{
			bool result = false;

			if (principal != null)
			{
				result = (bool)ObjectContextCache.Instance.GetOrAddNewValue(principal, (cache, key) =>
					{
						bool innerResult = false;

						string roleName = Configuration.AUConfigurationSection.GetConfig().MasterRoleFullCodeName;

						if (roleName.IsNotEmpty())
						{
							IRole role = new OguRole(roleName);

							if (role.ObjectsInRole.Count == 0)
								innerResult = true;
							else
								innerResult = principal.IsInRole(roleName);
						}
						else
							innerResult = true;

						cache.Add(key, innerResult);

						return innerResult;
					});
			}

			return result;
		}

		/// <summary>
		/// 是否拥有指定的权限
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="permissions"></param>
		/// <param name="permissionName"></param>
		/// <param name="containerIDs"></param>
		/// <returns></returns>
		public static bool HasPermissions(IPrincipal principal, SCContainerAndPermissionCollection permissions, string permissionName, params string[] containerIDs)
		{
			bool result = IsSupervisor(principal);

			if (result == false)
			{
				if (principal != null)
				{
					permissions.NullCheck("permissions");

					foreach (string containerID in containerIDs)
					{
						if (permissions.ContainsKey(containerID, permissionName))
						{
							result = true;
							break;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 是否拥有指定的权限
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="permissionName">权限的名称</param>
		/// <param name="containerIDs"></param>
		/// <returns></returns>
		public static bool HasPermissions(IPrincipal principal, string permissionName, params string[] containerIDs)
		{
			bool result = IsSupervisor(principal);

			if (result == false)
			{
				if (principal != null)
				{
					result = HasPermissions(principal, GetPrincipalPermissions(principal, containerIDs), permissionName, containerIDs);
				}
			}

			return result;
		}

		private static SCContainerAndPermissionCollection GetPrincipalPermissions(IPrincipal principal, params string[] containerIDs)
		{
			string calculatedKey = CalculatePrincipalAndPermissionKey(principal, containerIDs);

			return (SCContainerAndPermissionCollection)ObjectContextCache.Instance.GetOrAddNewValue(calculatedKey, (cache, key) =>
			{
				SCContainerAndPermissionCollection permissions = Adapters.AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(GetUserID(principal), containerIDs);

				cache.Add(key, permissions);

				return permissions;
			});
		}

		private static string CalculatePrincipalAndPermissionKey(IPrincipal principal, params string[] containerIDs)
		{
			StringBuilder strB = new StringBuilder();

			strB.Append(GetUserID(principal)).Append("-");

			foreach (string containerID in containerIDs)
			{
				strB.Append("-").Append(containerID);
			}

			return strB.ToString();
		}

		private static string GetUserID(IPrincipal principal)
		{
			string result = principal.Identity.Name;

			if (principal is DeluxePrincipal)
			{
				result = DeluxeIdentity.CurrentUser.ID;
			}

			return result;
		}
	}
}
