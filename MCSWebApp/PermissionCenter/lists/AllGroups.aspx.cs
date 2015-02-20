using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AllGroups : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "19DBD09A-A88D-4526-ACA1-6BD90ED5E4FD";

		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }
		}

		[Serializable]
		private class DeserialObject
		{
			public string[] SrcKeys { get; set; }

			public string[] TargetKeys { get; set; }
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

		protected DeluxeGrid CurrentGrid
		{
			get
			{
				switch (views.ActiveViewIndex)
				{
					case 0:
						return this.gridMain;
					default:
						return this.grid2;
				}
			}
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
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

				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
				this.grid2.PageSize = this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
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
						if (this.views.ActiveViewIndex != 0)
						{
							Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridMain);
							ProfileUtil.ToggleGeneralBrowseMode(0);
						}

						break;
					default:
						if (this.views.ActiveViewIndex != 1)
						{
							Util.SwapGrid(this.views, 1, this.CurrentGrid, this.grid2);
							ProfileUtil.ToggleGeneralBrowseMode(1);
						}

						break;
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
			this.dataSourceMain.LastQueryRowCount = -1;
			this.CurrentGrid.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
			this.acceptedLimitList.Value = string.Empty;
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

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			this.DoDelete(this.CurrentGrid.SelectedKeys.ToArray());
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
			System.Data.DataView view = e.ReturnValue as System.Data.DataView;
			if (view != null)
			{
				HashSet<string> parentIds = new HashSet<string>();
				HashSet<string> deleteLimitIds = new HashSet<string>();
				foreach (System.Data.DataRow row in view.Table.Rows)
				{
					string parentID = (string)row["ParentID"];
					parentIds.Add(parentID);
				}

				this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, parentIds);
				foreach (System.Data.DataRow row in view.Table.Rows)
				{
					string parentID = (string)row["ParentID"];
					if (this.IsDeleteEnabled(parentID))
						deleteLimitIds.Add((string)row["ID"]);
				}

				this.deleteLimitList.Value = JSONSerializerExecute.Serialize(deleteLimitIds);
			}
		}

		protected bool IsDeleteEnabled(string containerID)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, containerID, "DeleteChildren"));
		}

		protected void MoveToOrgClick(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();

			string[] orgKeys = this.GetPostedKeys();
			var groupKeys = this.CurrentGrid.SelectedKeys;

			if (orgKeys.Length == 1 && groupKeys.Count > 0)
			{
				this.DoMoveToOrg(orgKeys[0], groupKeys, this.notice.Errors);
			}
			else
			{
				this.notice.Text = "执行操作前只能选择一个组织，并且至少应选择1个群组";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void ProcessMoving(object sender, PostProgressDoPostedDataEventArgs e)
		{
			try
			{
				string ser = (string)e.Steps[0];

				DeserialObject obj = JSONSerializerExecute.Deserialize<DeserialObject>(ser);

				var executer = new BatchGroupTransferExecutor(obj.SrcKeys, obj.TargetKeys);

				executer.Execute();
			}
			catch (Exception ex)
			{
				ProcessProgress.Current.Output.WriteLine(ex.ToString());
			}

			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
			e.Result.CloseWindow = false;
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor executor = new ImportExecutor(file, result);

			executor.AddAction(new AllGroupImportAction()
			{
				IncludeConditions = Request.Form["includeConditionMembers"] == "includeConditionMembers",
				IncludeMembers = Request.Form["includeConstMembers"] == "includeConstMembers"
			});
			executor.Execute();
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/AllGroupUploadTemplate.htm");
		}

		private string[] GetPostedKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
		}

		private void DoMoveToOrg(string orgKey, List<string> groupKeys, IList<object> errors)
		{
			try
			{
				PC.SCOrganization org = (PC.SCOrganization)DbUtil.GetEffectiveObject(orgKey, string.Format("指定的组织(ID:{0})无效", orgKey));
				var grps = DbUtil.LoadObjects(groupKeys.ToArray());

				foreach (PC.SCBase item in grps)
				{
					try
					{
						if (item is PC.SCGroup && item.Status == SchemaObjectStatus.Normal)
						{
							PC.Executors.SCObjectOperations.InstanceWithPermissions.MoveObjectToOrganization(null, item, org);
						}
						else
						{
							errors.Add(string.Format("{0}不是有效的群组，已跳过", item.ToDescription()));
						}
					}
					catch (Exception ex)
					{
						WebUtility.ShowClientError(ex);
						errors.Add(ex);
					}
				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex);
				errors.Add(ex);
			}

			this.InnerRefreshList(); // 必须的
		}

		private void DoDelete(string[] keys)
		{
			try
			{
				Util.EnsureOperationSafe();

				var actor = SCObjectOperations.InstanceWithPermissions;
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("群组", errorAdapter, keys);

				foreach (PC.SCGroup group in objects)
				{
					actor.DeleteGroup(group, null, false);
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