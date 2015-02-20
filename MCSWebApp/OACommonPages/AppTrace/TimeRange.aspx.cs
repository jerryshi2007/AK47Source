using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class TimeRange : System.Web.UI.Page
	{
		[Serializable]
		internal class SearchCondition
		{
			[ConditionMapping("START_TIME", ">=")]
			public DateTime TimeFrom { get; set; }

			[ConditionMapping("START_TIME", "<", AdjustDays = 1)]
			public DateTime TimeTo { get; set; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (this.IsPostBack == false)
				this.views.ActiveViewIndex = 0;
		}

		private SearchCondition TimeCondition
		{
			get
			{
				return this.ViewState["TimeCondition"] as SearchCondition;
			}

			set
			{
				this.ViewState["TimeCondition"] = value;
			}
		}

		protected void DoContinue(object sender, EventArgs e)
		{
			this.TimeCondition = new SearchCondition() { TimeFrom = this.timeFrom.Value, TimeTo = this.timeTo.Value };
			this.views.ActiveViewIndex = 1;
			WhereSqlClauseBuilder where = ConditionMapping.GetWhereSqlClauseBuilder(this.TimeCondition);

			string[] ids = TimeRangeDataSource.QueryGuidsByCondition(where);

			this.postVal.Value = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(ids);

			this.lblCount.InnerText = ids.Length.ToString("#,##0");

			this.btnRecalc.Visible = ids.Length > 0;

			//this.grid.InitialData = new TimeRangeDataSource().Query(0, 0, builder, "CREATE_TIME", ref total);
		}

		protected void Regen_ExecuteStep(object data)
		{
			string processID = (string)data;

			try
			{
				IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

				WfPersistQueue pq = WfPersistQueue.FromProcess(process);

				WfPersistQueueAdapter.Instance.DoQueueOperation(pq);
			}
			catch (System.Exception ex)
			{
				string message = string.Format("生成流程{0}的数据异常: {1}", processID, ex.Message);

				throw new ApplicationException(message, ex);
			}
		}

		protected void RegenProcesses_Error(Exception ex, object data, ref bool isThrow)
		{
			isThrow = false;
		}

		protected void Back(object sender, EventArgs e)
		{
			this.views.ActiveViewIndex = 0;
		}
	}
}