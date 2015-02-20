using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AllMembers : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "152A167C-756B-4FEE-AEB0-26616C78060D";

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			private IOguObject owner;

			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }

			[ConditionMapping("AccountDisabled")]
			public bool AccountDisabled { get; set; }

			[ConditionMapping("WP", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string WorkPhone { get; set; }

			[ConditionMapping("MP", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string MobilePhone { get; set; }

			[SubConditionMapping("ID", "OwnerID")]
			public IOguObject Owner
			{
				get
				{
					return this.owner;
				}

				set
				{
					this.owner = value != null ? OguBase.CreateWrapperObject(value) : null;
				}
			}

			/// <summary>
			/// 无组织
			/// </summary>
			[NoMapping]
			public bool Dissociated { get; set; }
		}

		private SCContainerAndPermissionCollection containerPermissions = null;

		public bool DeleteEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode;
			}
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private DeluxeGrid CurrentGrid
		{
			get
			{
				if (views.ActiveViewIndex == 0)
					return this.gridViewUsers;
				else
					return this.gridView2;
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				this.shortcut.Visible = this.lnkNew.Enabled = false;
				this.btnDeleteSelected.Enabled = false;
				this.btnImport.Enabled = false;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();
				this.hfViewMode.Value = ProfileUtil.UserBrowseModeIndex.ToString();
				this.views.ActiveViewIndex = this.hfViewMode.Value == "2" ? 1 : 0;

				this.gridView2.PageSize = ProfileUtil.PageSize;
				this.gridViewUsers.PageSize = ProfileUtil.PageSize;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dd list-cmd shortcut";
					this.displayFilter.InnerText = "常规列表";
					break;
				case "1":
					this.listContainer.Attributes["class"] = "pc-grid-container pc-reduced-view";
					this.lnkDisplay.CssClass = "pc-toggler-dr list-cmd shortcut";
					this.displayFilter.InnerText = "精简列表";
					break;
				case "2":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dt list-cmd shortcut";
					this.displayFilter.InnerText = "精简表格";
					break;
			}

			base.OnPreRender(e);
		}

		protected bool IsDeleteEnabled(string ownerID)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || (ownerID != null && Util.ContainsPermission(this.containerPermissions, ownerID, "DeleteChildren")));
		}

		protected void ToggleViewMode(object sender, EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(0);
					break;
				case "1":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(1);
					break;
				case "2":
					Util.SwapGrid(this.views, 1, this.CurrentGrid, this.gridView2);
					ProfileUtil.ToggleUserBrowseMode(2);
					break;
			}
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			var keys = this.CurrentGrid.SelectedKeys;
			if (keys.Count > 0)
			{
				this.DoDelete(keys);
			}
			else
			{
				this.notice.Text = "至少应选择一个用户，才可以执行删除操作。";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void BatchAddToGroup(object sender, EventArgs e)
		{
			try
			{
				Util.EnsureOperationSafe();
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				SchemaObjectCollection groups = DbUtil.LoadAndCheckObjects("群组", errorAdapter, this.GetPostKeys());
				SchemaObjectCollection users = DbUtil.LoadAndCheckObjects("人员", errorAdapter, this.CurrentGrid.SelectedKeys.ToArray());

				AddUsersToGroups(users, groups);

				this.InnerRefreshList();
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex);
			}
		}

		protected void BatchAddToOrg(object sender, EventArgs e)
		{
			try
			{
				Util.EnsureOperationSafe();
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				SchemaObjectCollection orgs = DbUtil.LoadAndCheckObjects("组织", errorAdapter, this.GetPostKeys());
				SchemaObjectCollection users = DbUtil.LoadAndCheckObjects("人员", errorAdapter, this.CurrentGrid.SelectedKeys.ToArray());

				this.AddUsersToOrgs(users, orgs);
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex);
			}

			this.InnerRefreshList();
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		protected void ObjectDataSourceUsers_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.ObjectDataSourceUsers.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());

			if (Request.QueryString["dissociatedOnly"] == "1" || condition.Dissociated)
			{
				e.InputParameters["dissociatedOnly"] = true;
			}
		}

		protected void ObjectDataSourceUsers_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is DataView)
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					var view = (DataView)e.ReturnValue;
					List<string> ownerIds = new List<string>(view.Count);

					foreach (DataRow row in view.Table.Rows)
					{
						ownerIds.Add(row["OwnerID"].ToString());
					}

					this.containerPermissions = SCAclAdapter.Instance.GetCurrentContainerAndPermissions(Util.CurrentUser.ID, ownerIds);
				}
			}
		}

		protected void DoFileUpload(HttpPostedFile file, UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor exec = new ImportExecutor(file, result);

			exec.AddAction(new AllUserImportAction() { IncludeOrganizationRelation = Request.Form["includeOrg"] == "includeOrg", IncludeSecretaries = Request.Form["includeSecretaries"] == "includeSecretaries", IncludeGroupConstMembers = this.Request.Form["includeGroupMembers"] == "includeGroupMembers" });
			exec.Execute();
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/AllMembersUploadTemplate.htm");
		}

		private static void AddUsersToGroups(SchemaObjectCollection users, SchemaObjectCollection groups)
		{
			foreach (SCUser user in users)
			{
				foreach (SCGroup group in groups)
				{
					SCObjectOperations.InstanceWithPermissions.AddUserToGroup(user, group);
				}
			}
		}

		private void AddUsersToOrgs(SchemaObjectCollection users, SchemaObjectCollection orgs)
		{
			foreach (SCUser user in users)
			{
				foreach (SCOrganization org in orgs)
				{
					SCObjectOperations.InstanceWithPermissions.AddUserToOrganization(user, org);
				}
			}
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.ObjectDataSourceUsers.LastQueryRowCount = -1;
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

			// 说明：基本处理逻辑
			// 声明一个属性，可以是私有的，SearchCondition { get return WebUtils.GetViewStateValue("", ()null) set WebUtils.SetViewStateValue("", value) };
			// PageLoad时，if SearchCondition == null SearchCondition = new ();
			// searchBinding.Data = SearchCondition;
			// 最后在SearchButtonClick中，直接CollectData();
			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
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

				var actor = SCObjectOperations.InstanceWithPermissions;
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("人员", errorAdapter, keys.ToArray());

				foreach (SCUser user in objects)
				{
					actor.DeleteUser(user, null, false);
				}
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}

			this.InnerRefreshList();
		}
	}
}