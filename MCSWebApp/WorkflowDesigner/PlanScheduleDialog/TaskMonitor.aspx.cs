using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.WebControls;
using System.Text;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public partial class TaskMonitor : System.Web.UI.Page
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
			get { return (PageSearchCondition)ViewState["Condition"]; }

			set { this.ViewState["Condition"] = value; }
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
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
			this.DeluxeGrid1.PageIndex = 0;
			this.PreRender += new EventHandler(TaskMonitor_PreRender);
		}

		protected void DeluxeGrid1_PreRender(object sender, EventArgs e)
		{
			//HyperLinkField col = (HyperLinkField)DeluxeGrid1.Columns[9];
			//if (string.IsNullOrEmpty(this.Request.QueryString["traceurlfmt"]))
			//{
			//    col.Visible = false;
			//    col.DataNavigateUrlFormatString = "javascript:void(0);";
			//}
			//else
			//{
			//    col.Visible = true;
			//    col.DataNavigateUrlFormatString = this.Request.QueryString["traceurlfmt"];
			//}
		}

		protected void DeluxeGrid1_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Kill")
			{
				try
				{
					string taskId = e.CommandArgument.ToString();
					SysTask task = SysTaskAdapter.Instance.Load(taskId);
					if (task != null)
					{
						SysTaskAdapter.Instance.MoveToCompletedSysTask(taskId, SysTaskStatus.Aborted, "被用户终止");
					}
				}
				catch (Exception ex)
				{
					MCS.Web.Library.WebUtility.RegisterClientErrorMessage(ex);
				}

				this.InnerRefreshList();
			}
		}

		private void TaskMonitor_PreRender(object sender, EventArgs e)
		{
			this.DeluxeGrid1.DataBind();
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		protected void MoveAllSelectedClick(object sender, EventArgs e)
		{
			if (DeluxeGrid1.SelectedKeys.Count > 0)
			{
				try
				{
					ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

					ProcessProgress.Current.MinStep = 0;
					ProcessProgress.Current.MaxStep = DeluxeGrid1.SelectedKeys.Count;
					ProcessProgress.Current.CurrentStep = 0;

					ProcessProgress.Current.Response();

					foreach (var item in DeluxeGrid1.SelectedKeys)
					{
						SysTaskAdapter.Instance.MoveToCompletedSysTask(item, SysTaskStatus.Aborted, "被用户终止");

						ProcessProgress.Current.Increment();
						ProcessProgress.Current.Response();
					}
				}
				catch (System.Exception ex)
				{
					WebUtility.ResponseShowClientErrorScriptBlock(ex);
				}
				finally
				{
					this.ResponseCompletedScript();
				}
			}
		}

		protected void ExecuteAllSelectedClick(object sender, EventArgs e)
		{
			if (DeluxeGrid1.SelectedKeys.Count > 0)
			{
				try
				{
					ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

					ProcessProgress.Current.MinStep = 0;
					ProcessProgress.Current.MaxStep = DeluxeGrid1.SelectedKeys.Count;
					ProcessProgress.Current.CurrentStep = 0;

					ProcessProgress.Current.Response();

					foreach (var item in DeluxeGrid1.SelectedKeys)
					{
						using (var scope = TransactionScopeFactory.Create())
						{
							var task = SysTaskAdapter.Instance.Load(item);
							task.IsNotNull(i => SysTaskSettings.GetSettings().GetExecutor(task.TaskType).Execute(task));

							scope.Complete();
						}

						ProcessProgress.Current.Increment();
						ProcessProgress.Current.Response();
					}

					this.InnerRefreshList();
				}
				catch (System.Exception ex)
				{
					WebUtility.ResponseShowClientErrorScriptBlock(ex);
				}
				finally
				{
					this.ResponseCompletedScript();
				}
			}
		}

		protected void ExecuteAllClick(object sender, EventArgs e)
		{
			try
			{
				ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

				SysTaskCollection tasks = SysTaskAdapter.Instance.FetchNotRuningSysTasks(20, null);

				ProcessProgress.Current.MinStep = 0;
				ProcessProgress.Current.MaxStep = tasks.Count;
				ProcessProgress.Current.CurrentStep = 0;

				ProcessProgress.Current.Response();

				while (tasks.Count > 0)
				{
					foreach (SysTask task in tasks)
					{
						SysTaskSettings.GetSettings().GetExecutor(task.TaskType).Execute(task);

						ProcessProgress.Current.Increment();
						ProcessProgress.Current.Response();
					}

					tasks = SysTaskAdapter.Instance.FetchNotRuningSysTasks(20, null);

					ProcessProgress.Current.MaxStep += tasks.Count;
					ProcessProgress.Current.Response();
				}

				this.InnerRefreshList();
			}
			catch (System.Exception ex)
			{
				WebUtility.ResponseShowClientErrorScriptBlock(ex);
			}
			finally
			{
				this.ResponseCompletedScript();
			}
		}

		private void ResponseCompletedScript()
		{
			StringBuilder strB = new StringBuilder();

			strB.Append("<script type=\"text/javascript\">");
			strB.Append("\n" + SubmitButton.GetResetAllParentButtonsScript(false));
			strB.AppendFormat("\nparent.document.getElementById(\"{0}\").click();", "RefreshButton");
			strB.Append("</script>");

			this.Response.Write(strB.ToString());
			this.Response.End();
		}
	}
}