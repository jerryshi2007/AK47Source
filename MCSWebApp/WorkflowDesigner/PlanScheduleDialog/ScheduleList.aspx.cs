using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using System.Web.Services;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public class SimpleSchedule
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }

		public SimpleSchedule()
		{
		}

		public SimpleSchedule(JobSchedule jobSchedule)
		{
			this.ID = jobSchedule.ID;
			this.Name = jobSchedule.Name;
			this.Description = jobSchedule.Description;
			this.Enabled = jobSchedule.Enabled;
			this.StartTime = jobSchedule.StartTime;
			this.EndTime = jobSchedule.EndTime;
		}
	}

	[Serializable]
	public sealed class SimpleScheduleCondition
	{
		[ConditionMapping("SCHEDULE_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
		public string Name { get; set; }

		//[ORFieldMapping("ENABLED")]
		//public bool Enabled { get; set; }

		[ConditionMapping("START_TIME", ">=")]
		public DateTime StartDate { get; set; }

		[ConditionMapping("START_TIME", "<", AdjustDays = 1)]
		public DateTime StartDateEndDate { get; set; }

		[ConditionMapping("END_TIME", ">=")]
		public DateTime EndTimeStartDate { get; set; }

		[ConditionMapping("END_TIME", "<", AdjustDays = 1)]
		public DateTime EndTime { get; set; }
	}

	public partial class ScheduleList : System.Web.UI.Page
	{
		private static readonly SimpleSchedule[] EmptyResult = new SimpleSchedule[0];

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				this.QueryCondition = new SimpleScheduleCondition();
			}

			bool showEditBtn = WebUtility.GetRequestQueryValue<bool>("showEditBtn", true);//WebUtility.GetRequestQueryString<bool>("showEditBtn", true);
			if (showEditBtn)
			{
				btnOK.Visible = true;
				btnCancel.Visible = true;
				trBottom.Visible = true;
			}

			this.bindingControl.Data = this.QueryCondition;

			this.Response.CacheControl = "no-cache";
		}

		[WebMethod]
		public static SimpleSchedule[] QuerySchedules(string[] ids)
		{
			SimpleSchedule[] result = EmptyResult;
			if (ids.Length > 0)
			{
				var item = JobScheduleAdapter.Instance.LoadByInBuilder(m => { m.DataField = "SCHEDULE_ID"; m.AppendItem(ids); });
				if (item != null && item.Count > 0)
				{
					result = new SimpleSchedule[item.Count];
					for (int i = item.Count - 1; i >= 0; i--)
					{
						result[i] = new SimpleSchedule(item[i]);
					}
				}
			}

			return result;
		}

		[WebMethod]
		public static string QuerySchedulesJson(string[] ids)
		{
			SimpleSchedule[] result = EmptyResult;
			if (ids.Length > 0)
			{
				var item = JobScheduleAdapter.Instance.LoadByInBuilder(m => { m.DataField = "SCHEDULE_ID"; m.AppendItem(ids); });
				if (item != null && item.Count > 0)
				{
					result = new SimpleSchedule[item.Count];
					for (int i = item.Count - 1; i >= 0; i--)
					{
						result[i] = new SimpleSchedule(item[i]);
					}
				}
			}

			return MCS.Web.Library.Script.JSONSerializerExecute.Serialize(result);
		}

		protected SimpleScheduleCondition QueryCondition
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "QueryCondition", (SimpleScheduleCondition)null);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "QueryCondition", value);
			}
		}

		protected void RefreshList(object sender, EventArgs e)
		{
			this.InnerRefreshList();
		}

		private void InnerRefreshList()
		{
			// 重新刷新列表
			this.gridMain.PageIndex = 0;
			this.dataSourceMain.LastQueryRowCount = -1;
			this.gridMain.SelectedKeys.Clear();
			this.Page.PreRender += new EventHandler(this.DelayRefreshList);
		}

		private void DelayRefreshList(object sender, EventArgs e)
		{
			this.gridMain.DataBind();
		}

		protected void ScheduleDeluxeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{

		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			var selectedSchedule = this.hiddenSelectedSchedule.Value.Split(',');
			string[] result = JobScheduleAdapter.Instance.Delete(selectedSchedule);
			var deletedSchedule = selectedSchedule.Except(result).ToArray();
			string alertMsg = result.Length == 0 ? "alert('删除成功!');" : string.Format("alert('删除不成功。ID为({0})的计划因被使用，无法删除。');", string.Join(",", result));

			Page.ClientScript.RegisterStartupScript(this.GetType(), "deleteSchedule",
			alertMsg, true);

			this.InnerRefreshList();
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			this.bindingControl.CollectData(true);
			this.QueryCondition = this.bindingControl.Data as SimpleScheduleCondition;
			this.InnerRefreshList();
		}

		protected void DeluxeObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			if (e.ExecutingSelectCount == false)
			{
				WhereSqlClauseBuilder mappingbuilder = this.QueryCondition != null ? ConditionMapping.GetWhereSqlClauseBuilder(this.QueryCondition) : new WhereSqlClauseBuilder();
				WhereSqlClauseBuilder where2 = (WhereSqlClauseBuilder)new WhereSqlClauseBuilder().AppendItem("SCHEDULE_ID", DBNull.Value, "IS NOT");

				ConnectiveSqlClauseCollection all = new ConnectiveSqlClauseCollection(mappingbuilder, where2);

				this.dataSourceMain.Condition = all;
			}
		}
	}
}