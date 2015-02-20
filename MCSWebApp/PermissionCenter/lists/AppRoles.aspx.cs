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
using System.Collections.Specialized;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AppRoles : Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "F7425C73-3719-4470-894E-B340268BE1EB";
		private SCContainerAndPermissionCollection containerPermissions = null;

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		protected bool DeleteRoleEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.AppObject.ID, "DeleteRoles"));
			}
		}

		protected bool EditRoleMembersEnabled
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || Util.ContainsPermission(this.containerPermissions, this.AppObject.ID, "ModifyMembersInRoles"));
			}
		}

		protected SCSimpleObject AppObject
		{
			get
			{
				return (SCSimpleObject)this.ViewState["AppObject"];
			}

			set
			{
				this.ViewState["AppObject"] = value;
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
			{
				var objId = this.AppObject.ID;

				this.lnkNewRole.Enabled &= Util.ContainsPermission(this.containerPermissions, objId, "AddRoles");
				this.btnDeleteSelected.Enabled &= Util.ContainsPermission(this.containerPermissions, objId, "DeleteRoles");
				this.btnImport.Enabled &= (Util.ContainsPermission(this.containerPermissions, objId, "AddRoles") && Util.ContainsPermission(this.containerPermissions, objId, "ModifyMembersInRoles"));
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string appID = GetAppIDFromRequestOrRedirect();

			this.ctlUpload.Tag = appID;

			Util.InitSecurityContext(this.notice);

			this.Page.Response.CacheControl = "no-cache";

			if (Page.IsPostBack == false)
			{
				this.AppObject = SchemaObjectAdapter.Instance.Load(appID).ToSimpleObject();

				if (this.AppObject == null && this.AppObject.Status != SchemaObjectStatus.Normal)
				{
					throw new ObjectNotFoundException("指定的应用不存在或已删除");
				}

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = ProfileUtil.PageSize;
			}

			this.binding1.Data = this.AppObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (TimePointContext.Current.UseCurrentTime && Util.SuperVisiorMode == false)
				this.containerPermissions = SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.AppObject.ID });
			base.OnPreRender(e);
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

		protected void BatchDelete(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();

			var actor = SCObjectOperations.InstanceWithPermissions;
			var adapter = SchemaObjectAdapter.Instance;
			foreach (string key in this.gridMain.SelectedKeys)
			{
				try
				{
					var role = (SCRole)adapter.Load(key);
					if (role == null || role.Status != SchemaObjectStatus.Normal)
						throw new InvalidOperationException("指定的角色无效");
					actor.DeleteRole(role);
				}
				catch (Exception ex)
				{
					this.notice.AddErrorInfo(ex);
					MCS.Web.Library.WebUtility.ShowClientError(ex);
				}
			}

			this.InnerRefreshList();
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

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
		}

		protected void DoFileUpload(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			var fileType = Path.GetExtension(file.FileName).ToLower();

			if (fileType != ".xml")
				throw new InvalidDataException("上传的文件类型错误");

			ImportExecutor exec = new ImportExecutor(file, result);
			exec.AddAction(new RoleImportAction(this.ctlUpload.Tag)
			{
				IncludeConstMembers = Request.Form["iRoleMembers"] == "iRoleMembers",
				IncludeConditions = Request.Form["iRoleConditions"] == "iRoleConditions",
				CopyMode = Request.Form["mergeMode"] == "copyMode",
				IncludeRoleDefinitions = Request.Form["iDef"] == "iDef"
			});
			exec.Execute();

			return;
		}

		protected void ctlUpload_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{
			e.Content = WebXmlDocumentCache.GetDocument("~/inc/RoleUploadTemplate.htm") + this.GetScript(this.ctlUpload.Tag);
		}

		private string GetScript(string appId)
		{
			return "<script type=\"text/javascript\">(function(){ document.getElementById('parentId').value='" + Util.HtmlAttributeEncode(appId) + "';})(); </script>";
		}

		private void DoDelete(IEnumerable<string> keys)
		{
			try
			{
				Util.EnsureOperationSafe();
				DbUtil.GetEffectiveObject(this.AppObject);

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("角色", errorAdapter, keys.ToArray());

				foreach (SCRole role in objects)
				{
					try
					{
						actor.DeleteRole(role);
					}
					catch (Exception ex)
					{
						this.notice.AddErrorInfo(string.Format("删除角色 {0} 时出错：{1}", role.DisplayName, ex.Message));
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

		/// <summary>
		/// 如果url参数中有app参数，则直接返回，否则检查有没有appCodeName参数，如果有，则拼接app参数直接redirect
		/// </summary>
		/// <returns></returns>
		private static string GetAppIDFromRequestOrRedirect()
		{
			string appID = WebUtility.GetRequestQueryString("app", string.Empty);

			if (appID.IsNullOrEmpty())
			{
				string appCodeName = WebUtility.GetRequestQueryString("appCodeName", string.Empty);

				appCodeName.IsNotEmpty().FalseThrow("app参数和appCodeName不能都为空");

				SchemaObjectBase app = SchemaObjectAdapter.Instance.LoadByCodeName(StandardObjectSchemaType.Applications.ToString(), appCodeName, SchemaObjectStatus.Normal, DateTime.MinValue);

				(app != null).FalseThrow("不能找到CodeName为{0}的应用", appCodeName);

				NameValueCollection requestParams = HttpContext.Current.Request.Url.GetUriParamsCollection();

				requestParams.RemoveKeys("appCodeName");
				requestParams["app"] = app.ID;

				string newUri = UriHelper.CombineUrlParams(HttpContext.Current.Request.Url.ToString(), requestParams);

				HttpContext.Current.Response.Redirect(newUri);
			}

			return appID;
		}
	}
}