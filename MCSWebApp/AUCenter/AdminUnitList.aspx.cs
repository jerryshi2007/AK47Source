using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Operations;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security;
using System.IO;
using MCS.Web.WebControls;
using MCS.Web.Library;
using MCS.Library.Principal;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AdminUnitList : System.Web.UI.Page, INormalSceneDescriptor
	{
		#region 高级查询

		private string ThisPageSearchResourceKey = "F0727D8B-4EBF-4A1B-8063-BE385366DE06";
		private MCS.Library.SOA.DataObjects.Security.Permissions.SCContainerAndPermissionCollection containerPermissions;
		private bool deleteEnabled = false;

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

		public void AfterNormalSceneApplied()
		{
			if (this.IsAdminMode() == false)
			{
				this.lnkAdd.Enabled = Util.ContainsPermission(this.containerPermissions, this.ParentID, "AddSubUnit");
				this.lnkMove.Enabled = this.lnkDelete.Enabled = Util.ContainsPermission(this.containerPermissions, this.ParentID, "DeleteSubUnit");
				this.btnImport.Enabled = false;
			}
		}

		protected string SchemaID
		{
			get { return this.ViewState["SchemaID"] as string; }

			set { this.ViewState["SchemaID"] = value; }
		}

		protected string SchemaName
		{
			get { return this.ViewState["SchemaName"] as string; }

			set { this.ViewState["SchemaName"] = value; }
		}

		protected string AdminRoleName
		{
			get { return this.ViewState["AdminRoleName"] as string; }

			set { this.ViewState["AdminRoleName"] = value; }
		}

		public string[] Scopes
		{
			get { return this.ViewState["Scopes"] as string[]; }

			set { this.ViewState["Scopes"] = value; }
		}

		protected string ParentID
		{
			get { return this.ViewState["ParentID"] as string; }

			set { this.ViewState["ParentID"] = value; }
		}

		protected string ParentName
		{
			get { return this.ViewState["ParentName"] as string; }

			set { this.ViewState["ParentName"] = value; }
		}

		protected bool DeleteEnabled
		{
			get
			{
				return this.deleteEnabled;
			}
		}

		protected bool IsAdminMode()
		{
			return Util.SuperVisiorMode || (string.IsNullOrEmpty(this.AdminRoleName) == false && DeluxePrincipal.Current.IsInRole(this.AdminRoleName));
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string schemaID = this.Request.QueryString["schemaId"];
			string parentID = this.Request.QueryString["parentId"];
			if (Page.IsPostBack == false)
			{
				AU.AUSchema schema;
				try
				{
					this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
					this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

					if (string.IsNullOrEmpty(parentID) == false)
					{
						// 如果指定了ParentID，则忽略请求提供的schemaID。
						var parent = DbUtil.GetEffectiveObject<AdminUnit>(parentID);
						schema = DbUtil.GetEffectiveObject<AUSchema>(parent.AUSchemaID);
						this.SchemaID = schema.ID;
						this.SchemaName = schema.GetQualifiedName();
						this.ParentName = parent.Name;
						this.ParentID = parent.ID;
					}
					else if (string.IsNullOrEmpty(schemaID) == false)
					{
						schema = DbUtil.GetEffectiveObject<AUSchema>(schemaID);
						this.SchemaID = schema.ID;
						this.SchemaName = schema.GetQualifiedName();
						this.ParentID = this.ParentName = null;
					}
					else
					{
						throw new HttpException("没有提供schemaId");
					}

					this.AdminRoleName = schema.MasterRole;
					this.Scopes = schema.Scopes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				}
				catch (ObjectNotFoundException)
				{
					this.Response.Redirect("~/Nomatch.aspx", true);
				}
			}

			this.schemaNameLabel.InnerText = this.SchemaName;

			this.lnkAdd.Attributes["data-schemaId"] = this.SchemaID;
			this.lnkAdd.Attributes["data-parentId"] = this.ParentID ?? string.Empty;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
			this.ctlUpload.Tag = this.ParentID ?? this.SchemaID;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (TimePointContext.Current.UseCurrentTime && this.IsAdminMode() == false && string.IsNullOrEmpty(this.ParentID) == false)
				this.containerPermissions = AU.Adapters.AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.ParentID });

			this.deleteEnabled = TimePointContext.Current.UseCurrentTime && (this.IsAdminMode() || Util.ContainsPermission(containerPermissions, this.ParentID, "DeleteSubUnit"));

			base.OnPreRender(e);
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			this.path.DataBind();
			this.hfSchemaID.Value = this.SchemaID;
			this.hfParentID.Value = this.ParentID;
			this.scopesRepeater.DataSource = this.Scopes;
			this.scopesRepeater.DataBind();
		}

		protected void DoMove(object sender, EventArgs e)
		{
			var targetUnit = DbUtil.GetEffectiveObject<SchemaObjectBase>(this.hfPostData.Value);
			var srcObjects = DbUtil.GetEffectiveObjects(gridMain.SelectedKeys.ToArray());

			int total = srcObjects.Count;
			int success = 0;
			try
			{
				foreach (AdminUnit adminUnit in srcObjects)
				{
					Facade.InstanceWithPermissions.MoveAdminUnit(adminUnit, targetUnit is AUSchema ? null : (AdminUnit)targetUnit);
					success++;
				}
			}
			catch (Exception ex)
			{
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}

			InnerRefreshList();
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

		protected string GetSchemaName(string key)
		{
			return SchemaDefine.GetSchemaConfig(key).Description;
		}

		protected void gridMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.InnerRefreshList();
			}
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.dataSourceMain.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
		}

		protected void navPathSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["unitID"] = this.ParentID;
			e.InputParameters["includingSelf"] = true;
			e.InputParameters["timePoint"] = DateTime.MinValue;
		}

		protected void DoDeleteProgress(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = ProcessProgress.Current.CurrentStep = 1;

			var objectsToDelete = DbUtil.GetEffectiveObjects((from string id in e.Steps select id).ToArray());
			try
			{
				string message = null;
				message = string.Format("准备删除{0}个对象...", objectsToDelete.Count);
				ProcessProgress.Current.Output.WriteLine(message);
				ProcessProgress.Current.StatusText = message;
				ProcessProgress.Current.MaxStep = objectsToDelete.Count + 1;
				ProcessProgress.Current.CurrentStep = 1;

				foreach (AdminUnit item in objectsToDelete)
				{
					message = string.Format("正在删除{0}", item.GetQualifiedName());
					ProcessProgress.Current.Output.WriteLine(message);
					ProcessProgress.Current.StatusText = message;
					ProcessProgress.Current.Response();
					Facade.InstanceWithPermissions.DeleteAdminUnit(item);
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

			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor executor = new ImportExecutor(file, result);

			executor.AddAction(new AdminUnitImportAction()
			{
				ParentID = this.ctlUpload.Tag,
				IncludeRoleMembers = Request.Form["includeSchemaRoles"] == "includeSchemaRoles",
				IncludeScopeConditions = Request.Form["includeScopeCondition"] == "includeScopeCondition",
				ImportSubUnits = Request.Form["deepImport"] == "deepImport"
			});

			executor.Execute();
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/AdminUnitUploadTemplate.htm");
		}
	}
}