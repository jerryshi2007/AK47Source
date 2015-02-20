using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using PCClient = MCS.Library.SOA.DataObjects.Security.Client;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace AUCenter
{
	public sealed class PCService
	{
		public static readonly PCService Instance = new PCService();
		private static readonly string[] stRoles = { "Roles" };

		PCService()
		{
		}

		public SchemaObjectCollection LoadRoleByIds(string[] ids)
		{
			SchemaObjectCollection result = new SchemaObjectCollection();
			if (ids.Length > 0)
			{
				var qe = PCClient.PermissionCenterQueryService.Instance.GetObjectsByIDs(ids, stRoles, true);

				foreach (PCClient.ClientSCRole role in qe)
				{
					result.Add(Convert<SCRole>(role));
				}
			}

			return result;
		}

		private static SchemaObjectBase Convert<T1>(PCClient.ClientSCBase obj) where T1 : SchemaObjectBase, new()
		{
			T1 t = new T1();
			t.ID = obj.ID;
			t.VersionStartTime = obj.VersionStartTime;
			t.VersionEndTime = obj.VersionEndTime;
			t.Status = (SchemaObjectStatus)obj.Status;

			foreach (var key in obj.Properties.GetAllKeys())
			{
				t.Properties[key].StringValue = obj.Properties[key].StringValue;
			}

			return t;
		}

		public IList<RoleDisplayItem> LoadRoleDisplayItemsByIds(string[] ids)
		{
			if (ids.Length > 0)
			{
				var qe = PCClient.PermissionCenterQueryService.Instance.GetRoleDisplayItems(ids);

				return (from r in qe
						select new RoleDisplayItem()
						{
							ApplicationDisplayName = r.ApplicationDisplayName,
							ApplicationID = r.ApplicationID,
							ApplicationName = r.ApplicationName,
							RoleCodeName = r.RoleCodeName,
							RoleDisplayName = r.RoleDisplayName,
							RoleID = r.RoleID,
							RoleName = r.RoleName
						}).ToList();
			}
			else
			{
				return new List<RoleDisplayItem>();
			}
		}
	}
}