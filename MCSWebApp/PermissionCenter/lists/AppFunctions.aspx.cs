using System;
using System.IO;
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

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AppFunctions : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "C555250A-2A4D-4EAD-80E3-96637F0EFB3D";
		private SCContainerAndPermissionCollection containerPermissions = null;

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
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.AppObject.ID, "DeletePermissions"));
			}
		}

		private SCSimpleObject AppObject
		{
			get
			{
				return (SCSimpleObject)ViewState["AppObject"];
			}

			set
			{
				this.ViewState["AppObject"] = value;
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				var appId = this.AppObject.ID;

				bool enableNewFun = false, enableDelete = false;
				enableNewFun = Util.ContainsPermission(this.containerPermissions, appId, "AddPermissions");
				enableDelete = Util.ContainsPermission(this.containerPermissions, appId, "DeletePermissions");

				this.lnkNewFun.Enabled = enableNewFun;
				this.btnDeleteSelected.Enabled = enableDelete;
				this.btnImport.Enabled = false;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
				this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.AppObject.ID });
			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string appId = Request.QueryString["app"];

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.AppObject = SchemaObjectAdapter.Instance.Load(appId).ToSimpleObject();

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.binding1.Data = this.AppObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
			this.ctlUpload.Tag = appId;
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

			//this.AdvanceSearchEnabled = this.DeluxeSearch.IsAdvanceSearching;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			(sender as LinkButton).Attributes["data-parentid"] = this.AppObject.ID;
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
			var appKeys = this.gridMain.SelectedKeys;
			if (appKeys.Count > 0)
			{
				this.DoDelete(appKeys.ToArray());
			}
			else
			{
				this.notice.Text = "必须至少选择一个功能，才可以执行此操作";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			//if (this.AdvanceSearchEnabled)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				if (this.IsPostBack == false && this.Request.QueryString["id"] != null)
				{
					builder.AppendItem("ID", this.Request.QueryString["id"]);
				}

				this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
			}
			//else
			//{
			//    this.dataSourceMain.Condition = this.DeluxeSearch.GetCondition();
			//}
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor exec = new ImportExecutor(file, result);

			exec.AddAction(new PermissionImportAction(this.ctlUpload.Tag)
			{
				ApplicationId = Request.Form["parentId"],
				CopyMode = Request.Form["mergeMode"] == "copyMode"
			});
			exec.Execute();

			return;
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/PermissionUploadTemplate.htm") + this.GetScript(this.ctlUpload.Tag);
		}

		private void DoDelete(string[] funKeys)
		{
			try
			{
				Util.EnsureOperationSafe();
				DbUtil.GetEffectiveObject(this.AppObject);

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;

				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("功能", errorAdapter, funKeys);

				foreach (SCPermission permission in objects)
				{
					try
					{
						actor.DeletePermission(permission);
					}
					catch (Exception ex)
					{
						this.notice.AddErrorInfo(string.Format("删除功能 {0} 时出错：{1}", permission.DisplayName, ex.Message));
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

		private string GetScript(string appId)
		{
			return "<script type=\"text/javascript\">(function(){ document.getElementById('parentId').value='" + Util.HtmlAttributeEncode(appId) + "';})(); </script>";
		}
	}
}