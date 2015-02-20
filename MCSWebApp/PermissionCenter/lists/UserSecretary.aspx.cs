using System;
using System.Collections.Generic;
using System.Data;
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
using MCS.Web.WebControls;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class UserSecretary : Page, ITimeSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "82D8E79C-4DE7-4307-957B-A48FC9E991ED";

		private bool hasSelfRight = false; // 表示是否对当前用户有修改权限
		private SCRelationObjectCollection relations = null; // 列表中出现的关系(仅当非管理权限时)
		private SCContainerAndPermissionCollection acls = null; // 列表中关系是否具有的权限（仅当非管理权限时）

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

		private SCSimpleObject UserObject
		{
			get { return (SCSimpleObject)this.ViewState["UserObject"]; }
			set { this.ViewState["UserObject"] = value; }
		}

		public bool Editable
		{
			get
			{
				return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || this.hasSelfRight);
			}
		}

		string ITimeSceneDescriptor.NormalSceneName
		{
			get { return this.Editable ? "Normal" : "ReadOnly"; }
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
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

		protected bool IsDeleteEnabled(string userID)
		{
			return TimePointContext.Current.UseCurrentTime && (Util.SuperVisiorMode || (this.hasSelfRight && this.AclDetermineEditable(userID)));
		}

		private bool AclDetermineEditable(string userID)
		{
			bool result = false;

			if (this.relations != null && this.acls != null)
			{
				var myRelations = from r in this.relations where r.ID == userID select r.ParentID;

				foreach (var parentID in myRelations)
				{
					result |= Util.ContainsPermission(this.acls, parentID, "UpdateChildren");
				}
			}

			return result;
		}

		protected override void OnPreRender(EventArgs e)
		{
			var parentIds = DbUtil.LoadCurrentParentRelations(new string[] { this.UserObject.ID }, SchemaInfo.FilterByCategory("Organizations").ToSchemaNames()).ToParentIDArray();

			var containerPermissions = parentIds.Length > 0 ? SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, parentIds) : new SCContainerAndPermissionCollection();

			bool enabled = false;
			for (int i = parentIds.Length - 1; i >= 0; i--)
			{
				enabled |= Util.ContainsPermission(containerPermissions, parentIds[i], "UpdateChildren");
			}

			this.hasSelfRight = enabled;

			this.hfSelfID.Value = this.UserObject.ID;

			Util.ConfigToggleViewButton(this.views.ActiveViewIndex, this.lnkViewMode, this.lblViewMode);
			base.OnPreRender(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string userId = Request.QueryString["id"];

			this.Page.Response.CacheControl = "no-cache";

			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				var user = (SCUser)DbUtil.GetEffectiveObject(userId, null);

				this.UserObject = user.ToSimpleObject();

				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();

				this.gridMain.PageSize = this.grid2.PageSize = ProfileUtil.PageSize;
				this.views.ActiveViewIndex = ProfileUtil.GeneralViewModeIndex;
			}

			if (Request.QueryString["view"] == "boss")
			{
				this.tabBosses.Attributes.Add("class", "pc-active");
				this.grid2.GridTitle = this.gridMain.GridTitle = "上司列表";
			}
			else
			{
				this.tabSecretaries.Attributes.Add("class", "pc-active");
				this.grid2.GridTitle = this.gridMain.GridTitle = "秘书列表";
			}

			this.binding1.Data = this.UserObject;
			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected void HandleRowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DeleteItem")
			{
				this.DoDelete(new string[] { (string)e.CommandArgument });
			}
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

		protected void HandleMenuItemPreRender(object sender, EventArgs e)
		{
			(sender as LinkButton).Attributes["data-parentid"] = this.UserObject.ID;
		}

		protected void HandleAddUser(object sender, EventArgs e)
		{
			string[] keys = this.GetPostedKeys();

			if (keys.Length > 0)
			{
				try
				{
					Util.EnsureOperationSafe();
					var adapter = SchemaObjectAdapter.Instance;
					var myself = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);
					var errorAdapter = new ListErrorAdapter(this.notice.Errors);

					ISecretaryAction executor = this.GetAction();

					foreach (SCUser user in DbUtil.LoadAndCheckObjects("人员", errorAdapter, keys))
					{
						try
						{
							executor.DoAdd(user, myself);
						}
						catch (Exception ex)
						{
							this.notice.AddErrorInfo(ex);
							MCS.Web.Library.WebUtility.ShowClientError(ex);
						}
					}

					this.InnerRefreshList();
				}
				catch (Exception ex)
				{
					this.notice.AddErrorInfo(ex);
					MCS.Web.Library.WebUtility.ShowClientError(ex);
				}
			}
			else
			{
				this.notice.Text = "在执行操作前至少应选择一个项目";
				this.notice.RenderType = WebControls.NoticeType.Info;
			}
		}

		protected void BatchDelete(object sender, EventArgs e)
		{
			if (this.CurrentGrid.SelectedKeys.Count > 0)
			{
				this.DoDelete(this.CurrentGrid.SelectedKeys);
			}
			else
			{
				this.notice.Text = "操作前应至少选择一个项目";
				this.notice.RenderType = WebControls.NoticeType.Info;
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

			e.InputParameters["bossMode"] = this.Request.QueryString["view"] == "boss";
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is DataView && Util.SuperVisiorMode == false)
			{
				var view = e.ReturnValue as DataView;

				string[] ids = new string[view.Count];
				for (int i = 0; i < view.Count; i++)
				{
					ids[i] = view[i]["ID"].ToString();
				}

				this.relations = ids.Length > 0 ? DbUtil.LoadCurrentParentRelations(ids, SchemaInfo.FilterByCategory("Organizations").ToSchemaNames()) : new SCRelationObjectCollection();

				this.acls = this.relations.Count > 0 ? SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, this.relations.ToParentIDArray()) : new SCContainerAndPermissionCollection();
			}
		}

		private string[] GetPostedKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
		}

		private ISecretaryAction GetAction()
		{
			return Request.QueryString["view"] == "boss" ? (ISecretaryAction)new TopsAction() : new BottomsAction();
		}

		private void DoDelete(IEnumerable<string> keys)
		{
			try
			{
				Util.EnsureOperationSafe();
				var mySelf = (SCUser)DbUtil.GetEffectiveObject(this.UserObject);

				var actor = SCObjectOperations.InstanceWithPermissions;
				var adapter = SchemaObjectAdapter.Instance;
				ISecretaryAction executor = this.GetAction();
				var errorAdapter = new ListErrorAdapter(this.notice.Errors);

				var objects = DbUtil.LoadAndCheckObjects("人员", errorAdapter, keys.ToArray());

				foreach (SCUser user in objects)
				{
					try
					{
						executor.DoRemove(user, mySelf);
					}
					catch (Exception ex)
					{
						this.notice.AddErrorInfo(string.Format("指定秘书/上司关系人员 {0} 时出错：{1}", user.DisplayName, ex.Message));
						MCS.Web.Library.WebUtility.ShowClientError(ex);
					}

					this.InnerRefreshList();
				}
			}
			catch (Exception ex)
			{
				this.notice.AddErrorInfo(ex);
				MCS.Web.Library.WebUtility.ShowClientError(ex);
			}
		}

		#region 秘书操作

		/// <summary>
		/// 秘书操作的接口
		/// </summary>
		private interface ISecretaryAction
		{
			void DoRemove(SCUser otherUser, SCUser self);

			void DoAdd(SCUser otherUser, SCUser self);
		}

		/// <summary>
		/// 作为上司的操作
		/// </summary>
		private class TopsAction : ISecretaryAction
		{
			private ISCObjectOperations executor = SCObjectOperations.InstanceWithPermissions;

			public virtual void DoRemove(SCUser otherUser, SCUser self)
			{
				if (otherUser.ID == self.ID)
					throw new HttpException("不能指定自身为秘书");
				this.executor.RemoveSecretaryFromUser(self, otherUser);
			}

			public virtual void DoAdd(SCUser otherUser, SCUser self)
			{
				if (otherUser.ID == self.ID)
					throw new HttpException("不能指定自身为秘书");
				this.executor.AddSecretaryToUser(self, otherUser);
			}
		}

		/// <summary>
		/// 作为下级的操作
		/// </summary>
		private class BottomsAction : TopsAction
		{
			public override void DoRemove(SCUser otherUser, SCUser self)
			{
				base.DoRemove(self, otherUser);
			}

			public override void DoAdd(SCUser otherUser, SCUser self)
			{
				base.DoAdd(self, otherUser);
			}
		}

		#endregion
	}
}