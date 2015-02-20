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
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class GroupConstMembers : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "8C109926-27BF-4FF4-A5E6-99D05C4C3DFA";
		private SCContainerAndPermissionCollection containerPermissions;

		[Serializable]
		private class PageAdvancedSearchCondition
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

		protected bool CanEditMembers
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.GroupParentId, "EditMembersOfGroups"));
			}
		}

		private string GroupParentId
		{
			get
			{
				return (string)this.ViewState["GroupParentID"];
			}

			set
			{
				this.ViewState["GroupParentID"] = value;
			}
		}

		private SCSimpleObject GroupObject
		{
			get
			{
				return (SCSimpleObject)this.ViewState["SCGroup"];
			}

			set
			{
				this.ViewState["SCGroup"] = value;
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			this.lnkAddMember.Enabled = this.lnkAddAny.Enabled = this.btnDeleteSelected.Enabled = this.btnImport.Enabled = this.CanEditMembers;
			this.btnImport.Enabled = Util.SuperVisiorMode;
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

		protected override void OnPreRender(EventArgs e)
		{
			this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.GroupParentId });
			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string groupId = Request.QueryString["id"];

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.GroupObject = DbUtil.GetEffectiveObject(groupId, null).ToSimpleObject();
				this.GroupParentId = SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.GroupObject.ID).Find(m => m.Status == SchemaObjectStatus.Normal).ParentID;

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			this.binding1.Data = this.GroupObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void RefreshList(object sender, EventArgs e)
		{
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

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			(sender as LinkButton).Attributes["data-parentid"] = this.GroupObject.ID;
		}

		protected void HandleAddUser(object sender, EventArgs e)
		{
			try
			{
				string[] keys = this.GetPostedKeys();
				if (keys.Length > 0)
				{
					var adapter = SchemaObjectAdapter.Instance;
					var executor = SCObjectOperations.InstanceWithPermissions;

					var errorAdapter = new ListErrorAdapter(this.notice.Errors);

					Util.EnsureOperationSafe();
					var group = (SCGroup)DbUtil.GetEffectiveObject(this.GroupObject);

					var objects = DbUtil.LoadAndCheckObjects("人员", errorAdapter, keys);

					foreach (SCUser user in objects)
					{
						try
						{
							executor.AddUserToGroup(user, group);
						}
						catch (Exception ex)
						{
							this.notice.AddErrorInfo(string.Format("向群组添加人员 {0} 时发生错误：{1}", user.DisplayName, ex.Message));
							MCS.Web.Library.WebUtility.ShowClientError(ex);
						}
					}

					this.InnerRefreshList();
				}
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex.Message);
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			if (this.CurrentGrid.SelectedKeys.Count > 0)
			{
				this.DoDelete(this.CurrentGrid.SelectedKeys);
			}
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

		private string[] GetPostedKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
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

		private void DoDelete(IEnumerable<string> keys)
		{
			try
			{
				Util.EnsureOperationSafe();
				var grp = (SCGroup)DbUtil.GetEffectiveObject(this.GroupObject);

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("人员", errorAdapter, keys.ToArray());

				foreach (SCUser user in objects)
				{
					try
					{
						actor.RemoveUserFromGroup(user, grp);
					}
					catch (Exception ex)
					{
						this.notice.AddErrorInfo(string.Format("替群组移除人员 {0} 时出错：{1}", user.DisplayName, ex.Message));
						MCS.Web.Library.WebUtility.ShowClientError(ex);
					}
				}
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
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
			exec.AddAction(new GroupConstMembersImportAction(this.ctlUpload.Tag));
			exec.Execute();

			return;
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/CommonMemberUploadTemplate.htm") + this.GetScript(this.ctlUpload.Tag);
		}

		private string GetScript(string groupId)
		{
			return "<script type=\"text/javascript\">(function(){ document.getElementById('parentId').value='" + Util.HtmlAttributeEncode(groupId) + "';})(); </script>";
		}
	}
}