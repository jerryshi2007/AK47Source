using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Web.WebControls;

namespace MCS.OA.CommonPages.AsyncPersist
{
	public partial class ArchivedList : System.Web.UI.Page
	{
		#region 查询条件

		public static readonly string ThisPageSearchResourceKey = "C963BBC3-0C15-44C1-88F1-770EC4E92A0F";

		[Serializable]
		private class PageQueryCondition
		{
			[ConditionMapping("PROCESS_NAME", "LIKE", EscapeLikeString = true, Postfix = "%")]
			public string ProcessName { get; set; }

			[ConditionMapping("CREATOR_NAME", "LIKE", EscapeLikeString = true, Postfix = "%")]
			public string CreatorName { get; set; }

			[ConditionMapping("DEPARTMENT_NAME", "LIKE", EscapeLikeString = true, Postfix = "%")]
			public string Department { get; set; }

			[ConditionMapping("B.CREATE_TIME", ">=")]
			public DateTime CreationTimeFrom { get; set; }

			[ConditionMapping("B.CREATE_TIME", "<=")]
			public DateTime CreationTimeTo { get; set; }

			[ConditionMapping("START_TIME", ">=")]
			public DateTime StartTimeFrom { get; set; }

			[ConditionMapping("START_TIME", "<=")]
			public DateTime StartTimeTo { get; set; }
		}

		#endregion

		/// <summary>
		/// 获取或设置当前页的高级查询条件
		/// </summary>
		private PageQueryCondition CurrentAdvancedSearchCondition
		{
			get { return this.ViewState["AdvSearchCondition"] as PageQueryCondition; }

			set { this.ViewState["AdvSearchCondition"] = value; }
		}

		/// <summary>
		/// 在页面上下文中使用的查询条件
		/// </summary>
		public ConnectiveSqlClauseCollection Condition { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (this.Page.IsPostBack == false)
			{
				this.search1.UserCustomSearchConditions = Util.LoadSearchCondition(ThisPageSearchResourceKey, "Default");
				this.CurrentAdvancedSearchCondition = new PageQueryCondition();
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
			this.src1.LastQueryRowCount = -1;
			this.gridViewTask.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridViewTask.DataBind();
		}

		protected void SearchButtonClick(object sender, SearchEventArgs e)
		{
			this.gridViewTask.PageIndex = 0;
			this.searchBinding.CollectData();

			Util.SaveSearchCondition(e, this.search1, ThisPageSearchResourceKey, this.searchBinding.Data);
			this.InnerRefreshList();
		}

		protected void GridViewTask_RowDataBound(object sender, GridViewRowEventArgs e)
		{
		}

		protected void GridViewTask_ExportData(object sender, EventArgs e)
		{
		}

		protected void ObjectDataSourceSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (this.Condition == null)
			{
				var condition = this.CurrentAdvancedSearchCondition;

				WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(condition);

				this.Condition = new ConnectiveSqlClauseCollection(builder, this.search1.GetCondition());
			}

			this.src1.Condition = this.Condition;
		}
	}
}