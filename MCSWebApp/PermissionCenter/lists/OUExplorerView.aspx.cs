using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class OUExplorerView : Page, ITimeSceneDescriptor, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "6EADF811-9825-4410-A648-552BB5D6E49C";
		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("O.CodeName")]
			public string CodeName { get; set; }
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

		string ITimeSceneDescriptor.NormalSceneName
		{
			get
			{
				bool isInRoot = this.ParentOrganization.ID == SCOrganization.RootOrganizationID;
				return isInRoot ? "RootNormal" : "OrgNormal";
			}
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		#region 受保护的属性

		protected bool DeleteEnabled
		{
			get { return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.ParentOrganization.ID, "DeleteChildren")); }
		}

		protected bool UpdateEnabled
		{
			get { return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.ParentOrganization.ID, "UpdateChildren")); }
		}

		#endregion

		#region 私有的属性

		private SCSimpleObject ParentOrganization
		{
			get
			{
				return (SCSimpleObject)ViewState["ParentOrganization"];
			}

			set
			{
				this.ViewState["ParentOrganization"] = value;
			}
		}

		#endregion

		#region 受保护的方法

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				bool enableNewObj, enableDeleteObj, enableUpdate;

				this.btnImport.Enabled = false;
				this.btnImportAll.Enabled = false;
				if (this.ParentOrganization.ID == SCOrganization.RootOrganizationID)
				{
					enableUpdate = enableNewObj = enableDeleteObj = false;
				}
				else
				{
					enableNewObj = Util.ContainsPermission(this.containerPermissions, this.ParentOrganization.ID, "AddChildren");
					enableDeleteObj = Util.ContainsPermission(this.containerPermissions, this.ParentOrganization.ID, "DeleteChildren");
					enableUpdate = Util.ContainsPermission(this.containerPermissions, this.ParentOrganization.ID, "UpdateChildren");
				}

				this.lnkAddExistsMembers.Enabled = this.lnkNew.Enabled = this.shortCuts.Visible = this.lnkNewGroup.Enabled = this.lnkNewOrg.Enabled &= enableNewObj;
				this.btnDeleteSelected.Enabled &= enableDeleteObj;
				this.btnDeleteSelectedFull.Enabled &= enableDeleteObj;
				this.btnImportAll.Enabled = this.btnImport.Enabled &= enableNewObj & enableUpdate;
				this.hfDeleteEnabled.Value = enableDeleteObj ? "1" : string.Empty;
			}
			else
			{
				this.hfDeleteEnabled.Value = "1";
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.ParentOrganization.ID });

			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false && Page.IsCallback == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.grid2.PageSize = this.gridMain.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;

				if (this.InitState() == false)
				{
					this.Server.Transfer("~/lists/OUNotFound.aspx");
					return;
				}
			}

			this.binding1.Data = this.ParentOrganization;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void BatchTransfer(object sender, EventArgs e)
		{
			this.InnerRefreshOwnerTree();
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

		protected void ToggleVisibleClick(object sender, EventArgs e)
		{
			FilterSchemaType flags = this.GetCurrentFilterType();
			if (flags == FilterSchemaType.All)
			{
				this.displayFilter.InnerText = "所有类别";
			}
			else if (flags == 0)
			{
				this.displayFilter.InnerText = "所有类别";
				this.filterGroups.Checked = this.filterOrgs.Checked = this.filterUsers.Checked = true;
				flags = FilterSchemaType.All;
			}
			else
			{
				string text = string.Empty;
				if ((flags & FilterSchemaType.User) == FilterSchemaType.User)
				{
					text += "人员 ";
				}

				if ((flags & FilterSchemaType.Group) == FilterSchemaType.Group)
				{
					text += "群组 ";
				}

				if ((flags & FilterSchemaType.Organization) == FilterSchemaType.Organization)
				{
					text += "组织 ";
				}

				this.displayFilter.InnerText = text;
			}

			this.InnerRefreshList();
		}

		private FilterSchemaType GetCurrentFilterType()
		{
			FilterSchemaType flags = FilterSchemaType.None;
			flags |= this.filterGroups.Checked ? FilterSchemaType.Group : FilterSchemaType.None;
			flags |= this.filterOrgs.Checked ? FilterSchemaType.Organization : FilterSchemaType.None;
			flags |= this.filterUsers.Checked ? FilterSchemaType.User : FilterSchemaType.None;
			return flags;
		}

		protected void RefreshOwnerTree(object sender, EventArgs e)
		{
			this.InnerRefreshOwnerTree();
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

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			Util.EnsureOperationSafe();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			string oguId = Request.Form["parentId"];

			if (string.IsNullOrEmpty(oguId))
				throw new HttpException("没有使用parentId");

			PC.SCOrganization parent = oguId == PC.SCOrganization.RootOrganizationID ? PC.SCOrganization.GetRoot() : (PC.SCOrganization)DbUtil.GetEffectiveObject(oguId, "当前组织不存在或已删除");

			ImportExecutor executor = new ImportExecutor(file, result);

			if (Request.Form["includeOrg"] == "includeOrg")
				executor.AddAction(new OguOrganizationImportAction(parent) { });

			if (Request.Form["includeAcl"] == "includeAcl")
				executor.AddAction(new OguAclImportAction(parent) { });

			if (Request.Form["includeUser"] == "includeUser")
			{
				executor.AddAction(new OguUserImportAction(parent)
				{
					IncludeSecretaries = Request.Form["includeSecretary"] == "includeSecretary"
				});
			}

			if (Request.Form["includeGroup"] == "includeGroup")
			{
				executor.AddAction(new OguGroupImportAction(parent)
				{
					IncludeConditions = Request.Form["includeGroupConditions"] == "includeGroupConditions",
					IncludeMembers = Request.Form["includeGroupMembers"] == "includeGroupMembers"
				});
			}

			executor.Execute();
		}

		protected void DoDeepFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			if (Path.GetExtension(file.FileName).ToLower() != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			string oguId = Request.Form["parentId"];

			if (string.IsNullOrEmpty(oguId))
				throw new HttpException("没有使用parentId");

			Util.EnsureOperationSafe();

			PC.SCOrganization parent = oguId == PC.SCOrganization.RootOrganizationID ? PC.SCOrganization.GetRoot() : (PC.SCOrganization)DbUtil.GetEffectiveObject(oguId, "当前组织不存在或已删除");

			ImportExecutor executor = new ImportExecutor(file, result);

			if (Request.Form["includeAcl"] == "includeAcl")
				executor.AddAction(new OguAclImportAction(parent) { });

			if (Request.Form["includeUser"] == "includeUser")
			{
				executor.AddAction(new OguUserImportAction(parent)
				{
					IncludeSecretaries = Request.Form["includeSecretary"] == "includeSecretary"
				});
			}

			if (Request.Form["includeGroup"] == "includeGroup")
			{
				executor.AddAction(new OguGroupImportAction(parent)
				{
					IncludeConditions = Request.Form["includeGroupConditions"] == "includeGroupConditions",
					IncludeMembers = Request.Form["includeGroupMembers"] == "includeGroupMembers"
				});
			}

			executor.AddAction(new OguFullImportAction(parent)
			{
				IncludeOrganizations = this.Request.Form["includeOrg"] == "includeOrg",
				IncludeAcl = this.Request.Form["includeAcl"] == "includeAcl",
				IncludeUser = this.Request.Form["includeUser"] == "includeUser",
				IncludeSecretaries = this.Request.Form["includeSecretary"] == "includeSecretary",
				IncludeGroup = this.Request.Form["includeGroup"] == "includeGroup",
				IncludeGroupConditions = this.Request.Form["includeGroupConditions"] == "includeGroupConditions",
				IncludeGroupMembers = this.Request.Form["includeGroupMembers"] == "includeGroupMembers"
			});

			executor.Execute();
		}

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			(sender as LinkButton).Attributes["data-parentid"] = this.ParentOrganization.ID;
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var allConditions = new ConnectiveSqlClauseCollection(this.DeluxeSearch.GetCondition());

			var filter = this.GetCurrentFilterType();
			if (filter != FilterSchemaType.All)
			{
				int count = 0;
				var v = (int)filter;
				for (int i = sizeof(FilterSchemaType) * 8 - 1; i >= 0; i--)
				{
					if ((v & 1) == 1)
					{
						count++;
					}

					v >>= 1; // 计算有几个位被置一
				}

				string[] categories = new string[count];
				v = 0;

				if ((filter & FilterSchemaType.User) == FilterSchemaType.User)
				{
					categories[v++] = "Users";
				}

				if ((filter & FilterSchemaType.Group) == FilterSchemaType.Group)
				{
					categories[v++] = "Groups";
				}

				if ((filter & FilterSchemaType.Organization) == FilterSchemaType.Organization)
				{
					categories[v++] = "Organizations";
				}

				e.InputParameters["schemaTypes"] = SchemaInfo.FilterByCategory(categories).ToSchemaNames();
			}

			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				allConditions.Add(builder);
			}

			this.dataSourceMain.Condition = allConditions;
		}

		protected void AddExistMembers(object sender, EventArgs e)
		{
			try
			{
				Util.EnsureOperationSafe();

				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				SCOrganization parent = (SCOrganization)DbUtil.GetEffectiveObject(this.ParentOrganization);
				this.ParentOrganization = parent.ToSimpleObject();

				var users = DbUtil.LoadAndCheckObjects("人员", errorAdapter, this.GetPostedKeys());
				foreach (SCUser user in users)
				{
					try
					{
						SCObjectOperations.InstanceWithPermissions.AddUserToOrganization(user, parent);
					}
					catch (Exception ex)
					{
						this.notice.AddErrorInfo(string.Format("无法添加人员 {0} ：{1}", user.DisplayName, ex.Message));
						WebUtility.ShowClientError(ex);
					}
				}

				this.InnerRefreshList();
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex);
				this.notice.AddErrorInfo(ex);
			}
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/OUUploadTemplate.htm") + this.GetScript(this.ctlUpload.Tag);
		}

		protected void ctlFullUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/OUUploadTemplate.htm") + this.GetScript(this.ctlFullUpload.Tag);
		}

		protected void DoTransferProgress(object sender, PostProgressDoPostedDataEventArgs e)
		{
			try
			{
				string ser = (string)e.Steps[0];

				DeserialObject obj = JSONSerializerExecute.Deserialize<DeserialObject>(ser);

				BatchExecutor executor = null;

				switch (obj.ActionType)
				{
					case TransferActionType.UserCopyToGroup:
						executor = new CopyUsersToGroupsTransfer(obj.OrgKey, obj.SrcKeys, obj.TargetKeys);
						break;
					case TransferActionType.UserCopyToOrg:
						executor = new CopyUsersToOrgsTransfer(obj.OrgKey, obj.SrcKeys, obj.TargetKeys);
						break;
					case TransferActionType.UserMoveToOrg:
						executor = new MoveUsersToOrgsTransfer(obj.OrgKey, obj.SrcKeys, obj.TargetKeys);
						break;
					case TransferActionType.GroupMoveToOrg:
					case TransferActionType.OrgTransfer:
					case TransferActionType.MixedToOrg:
						executor = new MoveObjectsToOrgTransfer(obj.OrgKey, obj.SrcKeys, obj.TargetKeys);
						break;
					default:
						throw new InvalidOperationException("没有指定如何操作");
				}

				executor.Execute();
			}
			catch (Exception ex)
			{
				ProcessProgress.Current.Output.WriteLine(ex.ToString());
			}

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void gridMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "MoveUp":
					this.ReOrder((string)e.CommandArgument, false, false);
					break;
				case "MoveDown":
					this.ReOrder((string)e.CommandArgument, true, false);
					break;
				case "MoveTop":
					this.ReOrder((string)e.CommandArgument, false, true);
					break;
				case "MoveBottom":
					this.ReOrder((string)e.CommandArgument, true, true);
					break;
				default:
					break;
			}
		}

		protected void DoDeleteProgress(object sender, PostProgressDoPostedDataEventArgs e)
		{
			var parent = (SCOrganization)DbUtil.GetEffectiveObject(this.deleteProgress.Tag, null);
			DeleteProgressNextStep(e, parent);
		}

		protected void DoDeleteProgress2(object sender, PostProgressDoPostedDataEventArgs e)
		{
			var parent = (SCOrganization)DbUtil.GetEffectiveObject(this.deleteProgress2.Tag, null);
			DeleteProgressNextStep(e, parent);
		}


		protected void DoDeleteProgressFull(object sender, PostProgressDoPostedDataEventArgs e)
		{
			var parent = (SCOrganization)DbUtil.GetEffectiveObject(this.deleteFullProgress.Tag, null);

			DeleteProgressNextStepDestroyUsers(e, parent);
		}

		protected string SchemaTypeToString(string schemaName)
		{
			string result = schemaName;

			ObjectSchemaConfigurationElement schemaElement = ObjectSchemaSettings.GetConfig().Schemas[schemaName];

			if (schemaElement != null && schemaElement.Description.IsNotEmpty())
				result = schemaElement.Description;

			return result;
		}

		#endregion

		#region 私有的方法

		private static void DeleteProgressNextStep(PostProgressDoPostedDataEventArgs e, SCOrganization parent)
		{
			(e.Steps.Count > 0).FalseThrow<InvalidOperationException>("没有选择要删除的对象");

			var objectsToDelete = DbUtil.LoadObjects((from string id in e.Steps select id).ToArray());

			ProcessProgress.Current.Output.WriteLine("准备执行操作");
			PC.Executors.SCObjectOperations.InstanceWithPermissions.DeleteObjectsRecursively(objectsToDelete, parent);
			ProcessProgress.Current.Output.WriteLine("完毕");

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		private static void DeleteProgressNextStepDestroyUsers(PostProgressDoPostedDataEventArgs e, SCOrganization parent)
		{
			(e.Steps.Count > 0).FalseThrow<InvalidOperationException>("没有选择要删除的对象");

			var objectsToDelete = DbUtil.LoadObjects((from string id in e.Steps select id).ToArray());

			ProcessProgress.Current.Output.WriteLine("准备执行操作");
			PC.Executors.SCObjectOperations.InstanceWithPermissions.DeleteObjectsRecursively(objectsToDelete, parent);
			ProcessProgress.Current.Output.WriteLine("准备删除当前级别选定的人员……");

			foreach (var item in objectsToDelete)
			{
				if (item is PC.SCUser)
				{
					try
					{
						PC.Executors.SCObjectOperations.InstanceWithPermissions.DeleteUser((PC.SCUser)item, null, false);
						ProcessProgress.Current.Output.WriteLine("已删除用户{0}(ID:{1})", ((PC.SCUser)item).Name, item.ID); ;
					}
					catch (Exception ex)
					{
						ProcessProgress.Current.Output.WriteLine("删除用户{0}(ID:{1})时发生错误：{2}", ((PC.SCUser)item).Name, item.ID, ex.ToString()); ;
					}
				}
			}

			ProcessProgress.Current.Output.WriteLine("完毕");

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		private void InnerRefreshOwnerTree()
		{
			this.InnerRefreshList();
			var script = @"Sys.Application.add_init(function(){ refreshOwnerTree();});";
			Page.ClientScript.RegisterStartupScript(this.GetType(), "orgRefresh", script, true);
		}

		private void ReOrder(string objectId, bool down, bool toEdge)
		{
			try
			{
				Util.EnsureOperationSafe();
				DbUtil.ReOrder(objectId, this.ParentOrganization.ID, down, toEdge);
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				WebUtility.ShowClientError(ex);
			}

			this.InnerRefreshOwnerTree();
		}

		private string GetScript(string orgId)
		{
			return "<script type=\"text/javascript\">(function(){ document.getElementById('parentId').value='" + Util.HtmlAttributeEncode(orgId) + "';})(); </script>";
		}

		private bool InitState()
		{
			string ouID = WebUtility.GetRequestQueryString("ou", string.Empty);
			bool result = true;

			try
			{
				SchemaObjectBase parent = DbUtil.GetEffectiveObject(ouID, null);

				this.ParentOrganization = parent.ToSimpleObject();
				this.hfOuParentId.Value = parent.ID == SCOrganization.RootOrganizationID ? string.Empty : DbUtil.GetParentOURelation(parent.ID).ParentID;
			}
			catch (ObjectNotFoundException)
			{
				result = false;
			}

			return result;
		}

		private string[] GetPostedKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
		}

		#endregion

		[Serializable]
		private class DeserialObject
		{
			public TransferActionType ActionType { get; set; }

			public string[] SrcKeys { get; set; }

			public string[] TargetKeys { get; set; }

			public string OrgKey { get; set; }
		}

		[Flags]
		public enum FilterSchemaType
		{
			None = 0,
			User = 1,
			Group = 2,
			Organization = 4,
			All = User | FilterSchemaType.Group | FilterSchemaType.Organization
		}
	}
}