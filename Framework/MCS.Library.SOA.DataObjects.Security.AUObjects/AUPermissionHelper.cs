using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public static class AUPermissionHelper
	{
		public static bool IsSupervisor(this IPrincipal principal)
		{
			bool result = false;

			if (principal != null)
			{
				result = (bool)ObjectContextCache.Instance.GetOrAddNewValue(principal, (cache, key) =>
				{
					bool innerResult = false;
					string roleName = AUConfigurationSection.GetConfig().MasterRoleFullCodeName;

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

		private static PC.Permissions.SCContainerAndPermissionCollection GetPrincipalPermissions(IPrincipal principal, params string[] containerIDs)
		{
			string calculatedKey = CalculatePrincipalAndPermissionKey(principal, containerIDs);

			return (PC.Permissions.SCContainerAndPermissionCollection)ObjectContextCache.Instance.GetOrAddNewValue(calculatedKey, (cache, key) =>
			{
				PC.Permissions.SCContainerAndPermissionCollection permissions = null;
				permissions = AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(GetUserID(principal), containerIDs);

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

		/// <summary>
		/// 获取用户ID标识
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
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
