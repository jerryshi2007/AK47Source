using System;
using System.Collections.Generic;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Validators;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.Services
{
	/// <summary>
	/// CommonService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class CommonService : System.Web.Services.WebService
	{
		[WebMethod]
		public bool ValidateCodeNameUnique(string schemaType, string id, string parentID, string currentValue, bool includingDeleted)
		{
			return CodeNameUniqueValidatorFacade.Validate(currentValue, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue);
		}

		[WebMethod]
		public string GetPinYin(string schemaType, string id, string parentID, string currentValue, bool includingDeleted)
		{
			string result = string.Empty;

			List<string> strPinYin = SCSnapshotAdapter.Instance.GetPinYin(currentValue);

			if (strPinYin.Count > 0)
			{
				result = strPinYin[0];
				if (CodeNameUniqueValidatorFacade.Validate(result, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue) == false)
					result = GetCodeName(schemaType, id, parentID, result, includingDeleted, 1);
			}

			return result;
		}

		protected bool GetEditRoleMembersEnabled(string appID)
		{
			SCContainerAndPermissionCollection containerPermissions = null;
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
				containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { appID });
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(containerPermissions, appID, "ModifyMembersInRoles"));
		}

		[WebMethod]
		public List<AdditionOperation> GetAditionOperations(string id)
		{
			List<AdditionOperation> list = new List<AdditionOperation>();

			if (string.IsNullOrEmpty(id) == false)
			{
				var obj = SchemaObjectAdapter.Instance.Load(id);
				if (obj.Status == SchemaObjectStatus.Normal)
				{
					if (obj is SCUser)
					{
						var relation = obj.CurrentParentRelations.Find(m => m.Status == SchemaObjectStatus.Normal && m.Default && Util.IsOrganization(m.ParentSchemaType));
						if (relation != null)
							list.Add(new AdditionOperation("转到缺省组织", false, GetClientUrl("~/lists/OUExplorer.aspx?ou=" + relation.ParentID), "_blank"));
						string ownerId = ((SCUser)obj).OwnerID;
						if (string.IsNullOrEmpty(ownerId) == false)
						{
							list.Add(new AdditionOperation("转到所有者", false, GetClientUrl("~/lists/OUExplorer.aspx?ou=" + ownerId), "_blank"));
						}
					}
					else if (obj is SCGroup)
					{
						SCGroup grp = (SCGroup)obj;
						var parents = grp.CurrentParentRelations;
						var relation = parents.Find(m => m.Status == SchemaObjectStatus.Normal && Util.IsOrganization(m.ParentSchemaType));
						if (relation != null)
							list.Add(new AdditionOperation("转到组织", false, GetClientUrl("~/lists/OUExplorer.aspx?ou=" + relation.ParentID), "_blank"));
					}
					else if (obj is SCRole)
					{
						SCRole role = (SCRole)obj;
						list.Add(new AdditionOperation("打开应用", false, GetClientUrl("~/lists/AllApps.aspx?id=" + role.CurrentApplication.ID), "_blank"));
						list.Add(new AdditionOperation("定位", true, GetClientUrl("~/lists/AppRoles.aspx?app=" + role.CurrentApplication.ID) + "&id=" + role.ID, "_blank"));
						//if (this.GetEditRoleMembersEnabled(role.CurrentApplication.ID))
						//{
						//    list.Add(new AdditionOperation("角色矩阵", true, GetClientUrl("/MCSWebApp/WorkflowDesigner/MatrixModalDialog/RolePropertyExtension.aspx?AppID=" + role.CurrentApplication.ID + "&roleID=" + role.ID)));
						//}

						list.Add(new AdditionOperation("角色功能定义", true, GetClientUrl("~/dialogs/RoleDefinition.aspx?role=" + role.ID)));
					}
					else if (obj is SCPermission)
					{
						SCPermission permission = (SCPermission)obj;
						list.Add(new AdditionOperation("打开应用", false, GetClientUrl("~/lists/AllApps.aspx?id=" + permission.CurrentApplication.ID), "_blank"));
						list.Add(new AdditionOperation("定位", true, GetClientUrl("~/lists/AppFunctions.aspx?app=" + permission.CurrentApplication.ID) + "&id=" + permission.ID, "_blank"));
					}
					else if (obj is SCOrganization)
					{
						var relation = obj.CurrentParentRelations.Find(m => m.Status == SchemaObjectStatus.Normal && Util.IsOrganization(m.ParentSchemaType));
						if (relation != null)
							list.Add(new AdditionOperation("转到上级组织", false, GetClientUrl("~/lists/OUExplorer.aspx?ou=" + relation.ParentID), "_blank"));
					}
					else if (obj is SCApplication)
					{
						list.Add(new AdditionOperation("定位", false, GetClientUrl("~/lists/AllApps.aspx?id=" + obj.ID), "_blank"));
					}
				}
			}

			return list;
		}

		private string GetClientUrl(string relativeUrl)
		{
			return HttpContext.Current.Response.ApplyAppPathModifier(relativeUrl);
		}

		private string GetClientUrl(string relativeUrlFormat, params string[] args)
		{
			return HttpContext.Current.Response.ApplyAppPathModifier(string.Format(relativeUrlFormat, args));
		}

		private static string GetCodeName(string schemaType, string id, string parentID, string currentValue, bool includingDeleted, int index)
		{
			string result = string.Format("{0}{1}", currentValue, index);

			if (CodeNameUniqueValidatorFacade.Validate(result, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue) == false)
			{
				index++;
				result = GetCodeName(schemaType, id, parentID, currentValue, includingDeleted, index);
			}

			return result;
		}
	}
}
