using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class MemberOrgView : Page
	{
		public static readonly string ThisPageSearchResourceKey = "4DC39D82-E2D5-42FB-B4C3-C7DBB628D079";
		private SCContainerAndPermissionCollection containerPermissions;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }
		}

		//protected bool AdvanceSearchEnabled
		//{
		//    get
		//    {
		//        object o = this.ViewState["PageAdvanceSearch"];
		//        return (o is bool) ? (bool)o : false;
		//    }

		//    set
		//    {
		//        this.ViewState["PageAdvanceSearch"] = value;
		//    }
		//}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private SCSimpleObject DefaultOrg
		{
			get { return (SCSimpleObject)this.ViewState["DefaultOrg"]; }
			set { this.ViewState["DefaultOrg"] = value; }
		}

		private SCSimpleObject UserObject
		{
			get { return (SCSimpleObject)this.ViewState["UserObject"]; }
			set { this.ViewState["UserObject"] = value; }
		}

		private string OwnerID
		{
			get { return (string)this.ViewState["OwnerID"]; }
			set { this.ViewState["OwnerID"] = value; }
		}

		private string OwnerName
		{
			get { return (string)this.ViewState["OwnerName"]; }
			set { this.ViewState["OwnerName"] = value; }
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

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				ControllerHelper.ExecuteMethodByRequest(this);

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.binding1.Data = this.UserObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected bool IsDefaultOrg(string key)
		{
			return this.DefaultOrg == null ? false : this.DefaultOrg.ID == key;
		}

		protected bool IsOwner(string key)
		{
			return this.OwnerID == key && string.IsNullOrEmpty(this.OwnerID) == false;
		}

		protected bool IsSetDefaultEnabled(string targetID)
		{
			bool result = true;
			if (TimePointContext.Current.UseCurrentTime == false)
			{
				result = false;
			}
			else
			{
				if (Util.SuperVisiorMode)
				{
					result = true;
				}
				else
				{
					result = Util.ContainsPermission(this.containerPermissions, targetID, "UpdateChildren");
					if (result && this.DefaultOrg != null)
					{
						result = Util.ContainsPermission(this.containerPermissions, this.DefaultOrg.ID, "UpdateChildren");
					}
				}
			}

			return result;
		}

		protected bool IsSetOwnerEnabled(string targetID)
		{
			bool result = true;
			if (TimePointContext.Current.UseCurrentTime == false)
			{
				result = false;
			}
			else
			{
				if (Util.SuperVisiorMode)
				{
					result = true;
				}
				else
				{
					result = Util.ContainsPermission(this.containerPermissions, targetID, "AddChildren");
					if (result && string.IsNullOrEmpty(this.OwnerID) == false)
					{
						result = Util.ContainsPermission(this.containerPermissions, this.OwnerID, "DeleteChildren");
					}
					else
					{
						result = false; // 这样的情况下，只能等超级管理员来删
					}
				}
			}

			return result;
		}

		protected bool IsExitEnabled(string sourceID)
		{
			bool result = true;
			if (TimePointContext.Current.UseCurrentTime == false)
			{
				result = false;
			}
			else
			{
				if (Util.SuperVisiorMode)
				{
					result = true;
				}
				else
				{
					result = Util.ContainsPermission(this.containerPermissions, sourceID, "DeleteChildren");
				}
			}

			return result;
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "SetDefault":
					this.DoChangeOrg((string)e.CommandArgument);
					break;
				case "SetOwner":
					this.DoChangeOwner((string)e.CommandArgument);
					break;
				case "DeleteItem":
					this.DoRemoveFromOrg((string)e.CommandArgument);
					break;
				default:
					break;
			}
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

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
			}
			//else
			//{
			//    this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
			//}
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is System.Data.DataView)
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					var view = (DataView)e.ReturnValue;

					List<string> orgIds = new List<string>(view.Count + 1);
					foreach (DataRow row in view.Table.Rows)
					{
						orgIds.Add(row["ID"].ToString());
					}

					if (this.DefaultOrg != null)
					{
						orgIds.Add(this.DefaultOrg.ID);
					}

					this.containerPermissions = SCAclAdapter.Instance.GetCurrentContainerAndPermissions(Util.CurrentUser.ID, orgIds);
				}
			}
		}

		protected void ChangeOrg(object sender, EventArgs e)
		{
			var keys = this.CurrentGrid.SelectedKeys;

			if (keys.Count == 1)
			{
				this.DoChangeOrg(keys[0]);
			}
			else
			{
				this.notice.Text = "没有选择组织，或者选择了多个组织。";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		[ControllerMethod]
		protected void InitUserAndDefaultOrganization(string id)
		{
			SCUser user = (SCUser)SchemaObjectAdapter.Instance.Load(id);

			(user != null).FalseThrow<ObjectNotFoundException>("不能找到ID为{0}的用户", id);
			(user.Status == SchemaObjectStatus.Normal).FalseThrow<ObjectNotFoundException>("ID为{0}的用户的状态已经失效了", id);

			this.UserObject = user.ToSimpleObject();

			SCOrganization defOrg = (SCOrganization)user.CurrentParentRelations.GetDefaultParent();

			this.DefaultOrg = defOrg != null ? defOrg.ToSimpleObject() : null;

			this.OwnerID = user.OwnerID;
			this.OwnerName = user.OwnerName;
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.RenderOrganizationDisplay();
			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);
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

		private void RenderOrganizationDisplay()
		{
			this.lnkMainOrg.Text = Server.HtmlEncode(this.DefaultOrg != null ? this.DefaultOrg.DisplayName : "未知");
			this.lnkMainOrg.NavigateUrl = this.DefaultOrg != null ? "~/lists/OUExplorer.aspx?ou=" + Server.UrlEncode(this.DefaultOrg.ID) : string.Empty;

			this.lnkToOwner.Text = Server.HtmlEncode(this.OwnerName ?? string.Empty);
			this.lnkToOwner.NavigateUrl = "~/lists/OUExplorer.aspx?ou=" + this.OwnerID;
		}

		private void DoRemoveFromOrg(string orgKey)
		{
			try
			{
				var org = (SCOrganization)DbUtil.GetEffectiveObject(orgKey, string.Format("指定的组织(ID:{0})无效", orgKey));

				var user = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);

				SCObjectOperations.InstanceWithPermissions.DeleteObjectsRecursively(new SchemaObjectCollection() { user }, org);

				this.InitUserAndDefaultOrganization(this.UserObject.ID);
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				WebUtility.RegisterClientErrorMessage(ex);
			}

			this.InnerRefreshList();
		}

		private void DoChangeOrg(string orgKey)
		{
			try
			{
				var org = (SCOrganization)DbUtil.GetEffectiveObject(orgKey, string.Format("指定的组织(ID:{0})无效", orgKey));

				var user = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);

				SCObjectOperations.InstanceWithPermissions.SetUserDefaultOrganization(user, org);

				this.InitUserAndDefaultOrganization(this.UserObject.ID);
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				WebUtility.RegisterClientErrorMessage(ex);
			}

			this.InnerRefreshList();
		}

		private void DoChangeOwner(string orgKey)
		{
			try
			{
				var org = (SCOrganization)DbUtil.GetEffectiveObject(orgKey, string.Format("指定的组织(ID:{0})无效", orgKey));

				var user = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);

				SCObjectOperations.InstanceWithPermissions.ChangeOwner(user, org);

				this.InitUserAndDefaultOrganization(this.UserObject.ID);
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				WebUtility.RegisterClientErrorMessage(ex);
			}

			this.InnerRefreshList();
		}
	}
}