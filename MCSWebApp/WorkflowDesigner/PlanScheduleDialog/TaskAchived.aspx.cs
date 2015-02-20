using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public partial class TaskAchived : System.Web.UI.Page
	{
		[Serializable]
		public class PageSearchCondition
		{
			public PageSearchCondition()
			{
				this.StartTime = DateTime.MinValue;
				this.EndTime = DateTime.MinValue;
			}

			[ConditionMapping("START_TIME", Operation = ">=")]
			public DateTime StartTime { get; set; }

			[ConditionMapping("END_TIME", Operation = "<=")]
			public DateTime EndTime { get; set; }

			[ConditionMapping("TASK_TITLE", EscapeLikeString = true, Operation = "LIKE", Postfix = "%")]
			public string Title { get; set; }
		}


		internal PageSearchCondition Condition
		{
			get
			{
				return (PageSearchCondition)ViewState["Condition"];
			}

			set
			{
				this.ViewState["Condition"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false || this.EnableViewState == false)
			{
				this.Condition = new PageSearchCondition();

				lnk1.NavigateUrl = "TaskMonitor.aspx?" + Request.QueryString;
				lnk2.NavigateUrl = "TaskAchived.aspx?" + Request.QueryString;
			}

			this.searchBinding.Data = this.Condition;

			this.Response.CacheControl = "no-cache";
		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(this.Condition);

			string cate = Request.QueryString["category"];
			if (string.IsNullOrEmpty(cate) == false)
				builder.AppendItem("CATEGORY", cate);

			this.dataSourceMain.Condition = builder;

		}

		protected void SearchClick(object sender, EventArgs e)
		{
			this.searchBinding.CollectData();
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			this.dataSourceMain.LastQueryRowCount = -1;
			this.DeluxeGrid1.SelectedKeys.Clear();
			this.PreRender += new EventHandler(TaskAchived_PreRender);
		}

		void TaskAchived_PreRender(object sender, EventArgs e)
		{
			this.DeluxeGrid1.DataBind();
		}



		protected void DeluxeGrid1_PreRender(object sender, EventArgs e)
		{
		}



		protected void ReExecuteAllClick(object sender, EventArgs e)
		{
			if (DeluxeGrid1.SelectedKeys.Count > 0)
			{
				foreach (var item in DeluxeGrid1.SelectedKeys)
				{
					try
					{
						SysAccomplishedTaskAdapter.Instance.MoveToNoRunningTask(item);
					}
					catch (Exception ex)
					{
						MCS.Web.Library.WebUtility.RegisterClientErrorMessage(ex);
						break;
					}
				}
				this.InnerRefreshList();
			}
		}
	}
}