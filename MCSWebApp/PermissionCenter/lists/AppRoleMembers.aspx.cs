using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AppRoleMembers : Page, ITimeSceneDescriptor, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "BED73218-5623-41D7-8503-B7BAA34967A7";
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;
		private string appId = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }
		}

		[Serializable]
		public class RoleAndAppData
		{
			public string RoleID { get; set; }

			public string AppID { get; set; }

			public string AppCodeName { get; set; }

			public string AppDisplayName { get; set; }

			public string RoleName { get; set; }

			public string RoleCodeName { get; set; }

			public string RoleDisplayName { get; set; }

			public string VisibleName
			{
				get
				{
					if (string.IsNullOrEmpty(RoleDisplayName))
						return RoleName;
					else
						return RoleDisplayName;
				}
			}
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		string ITimeSceneDescriptor.NormalSceneName
		{
			get { return (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.appId, "ModifyMembersInRoles")) ? "Normal" : "ReadOnly"; }
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		protected bool DeleteEnabled
		{
			get { return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.appId, "ModifyMembersInRoles")); }
		}

		protected RoleAndAppData RoleAndAppObject
		{
			get
			{
				return (RoleAndAppData)this.ViewState["RoleAndAppObject"];
			}
			set
			{
				this.ViewState["RoleAndAppObject"] = value;
			}
		}

		protected bool EditRoleMembersEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.RoleAndAppObject.AppID, "ModifyMembersInRoles"));
			}
		}

		protected DeluxeGrid CurrentGrid
		{
			get
			{
				switch (this.views.ActiveViewIndex)
				{
					case 0:
						return this.gridMain;
					default:
						return this.grid2;
				}
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
			{
				var objId = this.RoleAndAppObject.AppID;

				this.btnImport.Enabled &= Util.ContainsPermission(this.containerPermissions, objId, "ModifyMembersInRoles");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Util.SuperVisiorMode == false)
			{
				this.appId = SCMemberRelationAdapter.Instance.LoadByMemberID(this.RoleAndAppObject.RoleID).Find(m => m.ContainerSchemaType == "Applications" && m.Status == SchemaObjectStatus.Normal).ContainerID;

				this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.appId });
			}

			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);

			roleMatrixEntryControl.AppID = this.RoleAndAppObject.AppID;
			roleMatrixEntryControl.AppCodeName = RoleAndAppObject.AppCodeName;
			roleMatrixEntryControl.AppName = RoleAndAppObject.AppDisplayName;
			roleMatrixEntryControl.RoleID = this.RoleAndAppObject.RoleID;
			roleMatrixEntryControl.RoleName = this.RoleAndAppObject.RoleName;
			roleMatrixEntryControl.RoleCodeName = this.RoleAndAppObject.RoleCodeName;
			roleMatrixEntryControl.RoleDescription = this.RoleAndAppObject.RoleDisplayName;
			roleMatrixEntryControl.ReadOnly = EditRoleMembersEnabled == false;

			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string roleId = Request.QueryString["role"];

			Util.InitSecurityContext(this.notice1);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				var role = (SCRole)DbUtil.GetEffectiveObject(roleId, null);
				var app = role.CurrentApplication;

				this.RoleAndAppObject = new RoleAndAppData() { AppID = app.ID, AppCodeName = app.CodeName, AppDisplayName = app.DisplayName, RoleCodeName = role.CodeName, RoleDisplayName = role.DisplayName, RoleID = role.ID, RoleName = role.Name };

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.binding1.Data = this.RoleAndAppObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.dataSourceMain.LastQueryRowCount = -1;
			this.CurrentGrid.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.CurrentGrid.DataBind();
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.CurrentGrid.PageIndex = 0;
			Util.UpdateSearchTip(this.DeluxeSearch);

			//this.AdvanceSearchEnabled = this.DeluxeSearch.IsAdvanceSearching;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void ToggleViewMode(object sender, CommandEventArgs e)
		{
			if (e.CommandName == "ToggleViewMode")
			{
				switch ((string)e.CommandArgument)
				{
					case "0":
						ProfileUtil.ToggleGeneralBrowseMode(0);
						Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridMain);
						break;
					default:
						ProfileUtil.ToggleGeneralBrowseMode(1);
						Util.SwapGrid(this.views, 1, this.CurrentGrid, this.grid2);
						break;
				}
			}
		}

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(this.RoleAndAppObject != null, "角色ID不得为空");
			(sender as LinkButton).Attributes["data-parentid"] = this.RoleAndAppObject.RoleID;
		}

		protected void AddMembersClick(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();

			string[] keys = this.GetPostKeys();
			if (keys != null && keys.Length > 0)
			{
				var role = (SCRole)SchemaObjectAdapter.Instance.Load(this.RoleAndAppObject.RoleID);
				if (role != null && role.Status == SchemaObjectStatus.Normal)
				{
					foreach (string key in keys)
					{
						try
						{
							var objLoaded = (SCBase)SchemaObjectAdapter.Instance.Load(key);

							SCObjectOperations.InstanceWithPermissions.AddMemberToRole(objLoaded, role);
						}
						catch (Exception ex)
						{
							this.notice1.AddErrorInfo(ex);
							WebUtility.ShowClientError(ex);
						}
					}
				}

				this.InnerRefreshList();
			}
		}

		protected void BatchRemove(object sender, EventArgs e)
		{
			if (this.CurrentGrid.SelectedKeys.Count > 0)
			{
				this.DoDelete(this.CurrentGrid.SelectedKeys);
			}
			else
			{
				this.notice1.Text = "执行删除前，至少应选择一个要删除的角色";
				this.notice1.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		private string[] GetPostKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
		}

		private void DoDelete(IEnumerable<string> keys)
		{
			try
			{
				Util.EnsureOperationSafe();
				var role = (SCRole)DbUtil.GetEffectiveObject(this.RoleAndAppObject.RoleID, null);
				var actor = SCObjectOperations.InstanceWithPermissions;
				var errorAdapter = new ListErrorAdapter(this.notice1.Errors);

				var objects = DbUtil.LoadAndCheckObjects("应用", errorAdapter, keys.ToArray());

				foreach (SCBase obj in objects)
				{
					actor.RemoveMemberFromRole(obj, role);
				}
			}
			catch (Exception ex)
			{
				this.notice1.Errors.Add(ex);
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}

			this.InnerRefreshList();
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor exec = new ImportExecutor(file, result);
			exec.AddAction(new RoleConstMembersImportAction(this.ctlUpload.Tag));
			exec.Execute();

			return;
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/CommonMemberUploadTemplate.htm") + this.GetScript(this.ctlUpload.Tag);
		}

		private string GetScript(string roleID)
		{
			return "<script type=\"text/javascript\">(function(){ document.getElementById('parentId').value='" + Util.HtmlAttributeEncode(roleID) + "';})(); </script>";
		}
	}
}