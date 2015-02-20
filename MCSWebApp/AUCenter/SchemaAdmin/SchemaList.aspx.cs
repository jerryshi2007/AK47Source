using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Operations;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using System.IO;
using MCS.Web.WebControls;
using MCS.Web.Library;

namespace AUCenter.SchemaAdmin
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class SchemaList : System.Web.UI.Page, INormalSceneDescriptor, ITimeSceneDescriptor
	{
		#region 高级查询
		private string ThisPageSearchResourceKey = "559D7DBB-1537-4199-A94E-DFE37B875D13";

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("S.CodeName")]
			public string CodeName { get; set; }
		}
		#endregion

		public bool EditEnabled
		{
			get
			{
				return Util.SuperVisiorMode && TimePointContext.Current.UseCurrentTime;
			}
		}

		public string NormalSceneName
		{
			get { return this.EditEnabled ? "Normal" : "ReadOnly"; }
		}

		public string ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (Util.SuperVisiorMode == false)
			{
				this.btnImport.Enabled = false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string cateID = this.Request.QueryString["category"];
			this.hfCategoryID.Value = cateID;
			if (Page.IsPostBack == false)
			{
				var cate = AU.Adapters.SchemaCategoryAdapter.Instance.LoadByID(cateID);
				if (cate != null)
					cateName.InnerText = cate.Name;
				else
					cateName.InnerText = "(未知分类)";

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();
			}

			this.lnkAdd.Attributes["data-category"] = cateID;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void SearchButtonClick(object sender, MCS.Web.WebControls.SearchEventArgs e)
		{
			this.gridMain.PageIndex = 0;

			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
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

		protected void gridMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				try
				{
					Util.EnsureOperationSafe();

					string id = (string)e.CommandArgument;

					var actor = Facade.InstanceWithPermissions;

					var schema = DbUtil.GetEffectiveObject<AUSchema>(id);

					actor.DeleteAdminSchema(schema);
				}
				catch (Exception ex)
				{
					MCS.Web.Library.WebUtility.ShowClientError(ex);
				}

				this.InnerRefreshList();
			}
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
		}

		protected void DoDeleteProgress(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = ProcessProgress.Current.CurrentStep = 1;

			if (e.Steps.Count > 0)
			{
				var objectsToDelete = DbUtil.GetEffectiveObjects((from string id in e.Steps select id).ToArray());
				try
				{
					string message = null;
					message = string.Format("准备删除{0}个对象...", objectsToDelete.Count);
					ProcessProgress.Current.Output.WriteLine(message);
					ProcessProgress.Current.StatusText = message;
					ProcessProgress.Current.MaxStep = objectsToDelete.Count + 1;
					ProcessProgress.Current.CurrentStep = 1;

					foreach (AUSchema item in objectsToDelete)
					{
						message = string.Format("正在删除{0}", item.GetQualifiedName());
						ProcessProgress.Current.Output.WriteLine(message);
						ProcessProgress.Current.StatusText = message;
						ProcessProgress.Current.Response();
						Facade.InstanceWithPermissions.DeleteAdminSchema(item);
						ProcessProgress.Current.CurrentStep++;
					}

					ProcessProgress.Current.StatusText = "完毕";
					ProcessProgress.Current.Output.WriteLine("完毕");
					ProcessProgress.Current.Response();
				}
				catch (Exception ex)
				{
					ProcessProgress.Current.Output.WriteLine(string.Format("操作遇到错误：\r\n{0}", ex.ToString()));
					ProcessProgress.Current.Response();
				}
			}

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor executor = new ImportExecutor(file, result);

			executor.AddAction(new AUSchemaImportAction()
			{
				IncludeSchemaRoles = Request.Form["includeSchemaRoles"] == "includeSchemaRoles",
				TargetCategory = Request.QueryString["category"]
			});
			executor.Execute();
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/SchemaListUploadTemplate.htm");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
	}
}