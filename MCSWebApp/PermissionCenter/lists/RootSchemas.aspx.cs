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
using MCS.Web.Library;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class RootSchemas : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "7D8A882B-F0BD-4B80-B093-34BC15358821";

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("O.CodeName")]
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
				this.btnNewRoot.Enabled = this.btnDeleteSelected.Enabled = this.btnImport.Enabled = false;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.InitSecurityContext(this.notice);

			this.hfOuId.Value = PC.SCOrganization.RootOrganizationID;

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

			//this.AdvanceSearchEnabled = this.DeluxeSearch.IsAdvanceSearching;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		protected void btnNewRoot_PreRender(object sender, EventArgs e)
		{
			LinkButton btn = sender as LinkButton;
			if (btn != null)
			{
				btn.Attributes.Add("data-parentid", SCOrganization.RootOrganizationID);
			}
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			if (this.gridMain.SelectedKeys.Count > 0)
			{
				this.DoDelete(this.gridMain.SelectedKeys.ToArray());
			}
			else
			{
				this.notice.Text = "在执行删除前至少应选择一个组织";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		protected void DoSortingList(object sender, GridViewSortEventArgs e)
		{
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

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/RootOUUploadTemplate.htm");
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor executor = new ImportExecutor(file, result);

			var rootOU = PC.SCOrganization.GetRoot();

			executor.AddAction(new OguOrganizationImportAction(rootOU) { });

			if (Request.Form["includeAcl"] == "includeAcl")
				executor.AddAction(new OguAclImportAction(rootOU) { });

			executor.Execute();
		}

		private void DoDelete(string[] keys)
		{
			try
			{
				Util.EnsureOperationSafe();

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);
				var root = SCOrganization.GetRoot();

				var objects = DbUtil.LoadAndCheckObjects("组织", errorAdapter, keys);

				try
				{
					actor.DeleteObjectsRecursively(objects, root);
				}
				catch (Exception ex)
				{
					this.notice.AddErrorInfo(ex.Message);
					MCS.Web.Library.WebUtility.ShowClientError(ex);
				}

				this.InnerRefreshList();
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}
		}
	}
}