using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AllApps : System.Web.UI.Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "E9D6BD46-D9E4-42FE-86E9-AA4724749CBB";

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

		protected bool DeleteEnabled
		{
			get { return TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode; }
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				this.lnkNewApp.Enabled = this.btnDeleteSelected.Enabled = this.btnImport.Enabled = false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			Util.InitSecurityContext(this.notice);
			base.OnLoad(e);

			if (Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();
				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

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
			this.gridMain.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridMain.DataBind();
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.gridMain.PageIndex = 0;
			Util.UpdateSearchTip(this.DeluxeSearch);

			//this.AdvanceSearchEnabled = true;//this.DeluxeSearch.IsAdvanceSearching;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			this.DoDelete(this.gridMain.SelectedKeys.ToArray());
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (this.IsPostBack == false && string.IsNullOrEmpty(this.Request.QueryString["id"]) == false)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("ID", this.Request.QueryString["id"]);

				this.dataSourceMain.Condition = where;
			}
			else
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
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor exec = new ImportExecutor(file, result);
			exec.AddAction(new AppImportAction()
			{
				CopyMode = Request.Form["mergeMode"] == "copyMode",
				IncludeRoles = Request.Form["iRoles"] == "iRoles",
				IncludeAcls = Request.Form["iAcl"] == "iAcl",
				IncludePermissions = Request.Form["iFun"] == "iFun",
				IncludeRoleDefinitions = Request.Form["iDef"] == "iDef",
				IncludeRoleMembers = Request.Form["iRoleMembers"] == "iRoleMembers",
				IncludeRoleConditions = Request.Form["iRoleConditions"] == "iRoleConditions"
			});
			exec.Execute();

			return;
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/AppUploadTemplate.htm");
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		private void DoDelete(string[] keys)
		{
			try
			{
				Util.EnsureOperationSafe();

				var actor = SCObjectOperations.InstanceWithPermissions;

				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("应用", errorAdapter, keys);

				foreach (SCApplication app in objects)
				{
					if (app.Status == SchemaObjectStatus.Normal)
					{
						var subObjects = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(app.ID).FilterByStatus(SchemaObjectStatusFilterTypes.Normal);

						try
						{
							bool anySubObjectsDeleted = false;

							foreach (var item in subObjects)
							{
								var member = item.Member;
								if (member != null && member.Status == SchemaObjectStatus.Normal)
								{
									if (member is SCRole)
									{
										PC.Executors.SCObjectOperations.InstanceWithPermissions.DeleteRole((SCRole)member);
										this.notice.AddErrorInfo(string.Format("已删除应用 {0} 中的角色 {1}", app.ToDescription(), ((SCRole)member).ToDescription()));
										anySubObjectsDeleted = true;
									}
									else if (member is SCPermission)
									{
										PC.Executors.SCObjectOperations.InstanceWithPermissions.DeletePermission((SCPermission)member);
										this.notice.AddErrorInfo(string.Format("已删除应用 {0} 中的功能 {1}", app.ToDescription(), ((SCPermission)member).ToDescription()));
										anySubObjectsDeleted = true;
									}
								}
							}

							actor.DeleteApplication(app);

							if (anySubObjectsDeleted)
							{
								this.notice.Errors.Add(string.Format("已删除应用 {0}。", app.ToDescription()));
							}
						}
						catch (Exception ex)
						{
							this.notice.Errors.Add(string.Format("因为出现了异常，因此未能删除应用 {0}，异常原因参考下一个消息。", app.ToDescription()));
							this.notice.Errors.Add(ex);
						}
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
	}
}