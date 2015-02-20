using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Web.WebControls;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class DeletedMemberList : System.Web.UI.Page, INormalSceneDescriptor
	{
		public static readonly string ThisPageSearchResourceKey = "4CC705BE-32E2-44C1-B9C1-9A0C5CD0BB34";

		[Serializable]
		internal class PageAdvancedSearchCondition
		{
			private IOguObject owner;

			[ConditionMapping("CodeName")]
			public string CodeName { get; set; }

			[ConditionMapping("AccountDisabled")]
			public bool AccountDisabled { get; set; }

			[ConditionMapping("WP", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string WorkPhone { get; set; }

			[ConditionMapping("MP", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string MobilePhone { get; set; }

			[SubConditionMapping("ID", "OwnerID")]
			public IOguObject Owner
			{
				get
				{
					return this.owner;
				}

				set
				{
					this.owner = value != null ? OguBase.CreateWrapperObject(value) : null;
				}
			}
		}

		private PageAdvancedSearchCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageAdvancedSearchCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		private DeluxeGrid CurrentGrid
		{
			get
			{
				if (views.ActiveViewIndex == 0)
					return this.gridViewUsers;
				else
					return this.gridView2;
			}
		}

		void INormalSceneDescriptor.AfterNormalSceneApplied()
		{

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.InitSecurityContext(this.notice);

			if (Page.IsPostBack == false)
			{
				this.DeluxeSearch.UserCustomSearchConditions = DbUtil.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageAdvancedSearchCondition();
				this.hfViewMode.Value = ProfileUtil.UserBrowseModeIndex.ToString();
				this.views.ActiveViewIndex = this.hfViewMode.Value == "2" ? 1 : 0;

				this.gridView2.PageSize = ProfileUtil.PageSize;
				this.gridViewUsers.PageSize = ProfileUtil.PageSize;
			}

			this.searchBinding.Data = this.CurrentAdvancedSearchCondition;
		}

		protected override void OnPreRender(EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dd list-cmd shortcut";
					this.displayFilter.InnerText = "常规列表";
					break;
				case "1":
					this.listContainer.Attributes["class"] = "pc-grid-container pc-reduced-view";
					this.lnkDisplay.CssClass = "pc-toggler-dr list-cmd shortcut";
					this.displayFilter.InnerText = "精简列表";
					break;
				case "2":
					this.listContainer.Attributes["class"] = "pc-grid-container";
					this.lnkDisplay.CssClass = "pc-toggler-dt list-cmd shortcut";
					this.displayFilter.InnerText = "精简表格";
					break;
			}

			base.OnPreRender(e);
		}

		protected void ToggleViewMode(object sender, EventArgs e)
		{
			switch (this.hfViewMode.Value)
			{
				default:
				case "0":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(0);
					break;
				case "1":
					Util.SwapGrid(this.views, 0, this.CurrentGrid, this.gridViewUsers);
					ProfileUtil.ToggleUserBrowseMode(1);
					break;
				case "2":
					Util.SwapGrid(this.views, 1, this.CurrentGrid, this.gridView2);
					ProfileUtil.ToggleUserBrowseMode(2);
					break;
			}
		}

		protected void ObjectDataSourceUsers_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var condition = this.CurrentAdvancedSearchCondition;

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

			this.ObjectDataSourceUsers.Condition = new ConnectiveSqlClauseCollection(builder, this.DeluxeSearch.GetCondition());
		}

		protected void ObjectDataSourceUsers_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			if (e.ReturnValue is DataView)
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					var view = (DataView)e.ReturnValue;
					List<string> ownerIds = new List<string>(view.Count);

					foreach (DataRow row in view.Table.Rows)
					{
						ownerIds.Add(row["OwnerID"].ToString());
					}
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
			this.ObjectDataSourceUsers.LastQueryRowCount = -1;
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

			// 说明：基本处理逻辑
			// 声明一个属性，可以是私有的，SearchCondition { get return WebUtils.GetViewStateValue("", ()null) set WebUtils.SetViewStateValue("", value) };
			// PageLoad时，if SearchCondition == null SearchCondition = new ();
			// searchBinding.Data = SearchCondition;
			// 最后在SearchButtonClick中，直接CollectData();
			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.DeluxeSearch, ThisPageSearchResourceKey, this.searchBinding.Data);

			this.InnerRefreshList();
		}

		private string[] GetPostKeys()
		{
			return this.actionData.Value.Split(Util.CommaSpliter, StringSplitOptions.RemoveEmptyEntries);
		}

		protected void Rebirth(object sender, EventArgs e)
		{
			if (Util.SuperVisiorMode)
			{
				if (this.CurrentGrid.SelectedKeys.Count > 0)
				{
					var objs = DbUtil.LoadObjectsIgnoreStatus(this.CurrentGrid.SelectedKeys.ToArray());

					foreach (var item in objs)
					{
						try
						{
							if (item is SCUser)
							{
								if (item.Status == SchemaObjectStatus.Normal)
									throw new InvalidOperationException(string.Format("{0} (ID:{1})为正常状态", ((PC.SCUser)item).Name, item.ID));
								item.Status = SchemaObjectStatus.Normal;
								PC.Executors.SCObjectOperations.InstanceWithPermissions.AddUser((SCUser)item, null);
							}
						}
						catch (Exception ex)
						{
							this.notice.AddErrorInfo(ex);
						}
					}

					this.InnerRefreshList();
				}
			}
			else
			{
				this.notice.Text = "需要管理员权限来执行此操作。";
			}
		}
	}
}