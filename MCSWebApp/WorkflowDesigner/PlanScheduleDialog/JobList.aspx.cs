using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using System.Collections;

namespace WorkflowDesigner.PlanScheduleDialog
{
	[Serializable]
	public sealed class JobCondition
	{
		[ConditionMapping("JOB_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
		public string Name { get; set; }

		[ConditionMapping("JOB_TYPE", EnumUsage = EnumUsageTypes.UseEnumValue)]
		public string JobType { get; set; }

        [ConditionMapping("LAST_START_EXE_TIME", ">=")]
		public DateTime LastExecuteStartTime { get; set; }

        [ConditionMapping("LAST_START_EXE_TIME", "<", AdjustDays = 1)]
		public DateTime LastExecuteEndTime { get; set; }
	}

	public partial class JobList : System.Web.UI.Page
	{
		static readonly EnumItemDescriptionList jobTypeDesc = EnumItemDescriptionAttribute.GetDescriptionList(typeof(JobType));

		public static EnumItemDescriptionList GetJobTypeSource()
		{
			return jobTypeDesc;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			bool showEditBtn = WebUtility.GetRequestQueryValue<bool>("showEditBtn", true);//WebUtility.GetRequestQueryString<bool>("showEditBtn", true);
			if (showEditBtn)
			{
				btnOK.Visible = true;
				btnCancel.Visible = true;
			}

			this.bindingControl.Data = this.QueryCondition;

			if (this.Page.IsPostBack == false)
				this.InitData();
		}

		private void InitData()
		{

			//this.dr_JobType.Items.Clear();
			//this.dr_JobType.AppendDataBoundItems = false;
			//this.dr_JobType.DataTextField = "Description";
			//this.dr_JobType.DataValueField = "EnumValue";
			//this.dr_JobType.DataSource = jobTypeDesc;
			//this.dr_JobType.DataBind();
		}

		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			var keys = JobDeluxeGrid.SelectedKeys.ToArray();
			if (keys.Length > 0)
			{
				try
				{
					var jobBases = JobBaseAdapter.Instance.LoadByInBuilder(inB => { inB.DataField = "JOB_ID"; inB.AppendItem(keys); });
					foreach (JobBase job in jobBases)
					{
						switch (job.JobType)
						{
							case JobType.InvokeService:
								InvokeWebServiceJobAdapter.Instance.Delete(new string[] { job.JobID });
								break;
							case JobType.StartWorkflow:
								StartWorkflowJobAdapter.Instance.Delete(new string[] { job.JobID });
								break;
							default:
								break;
						}
					}

					Page.ClientScript.RegisterStartupScript(this.GetType(), "deleteJob",
					string.Format("alert('删除成功!');"),
					true);
				}
				catch (Exception ex)
				{
					WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
				}

				this.LastQueryRowCount = -1;
			}
		}

		public void btnSearch_Click(object sender, EventArgs e)
		{
			this.bindingControl.CollectData(true);
			this.QueryCondition = this.bindingControl.Data as JobCondition;

			if (this.QueryCondition.JobType.IsNullOrEmpty())
				this.QueryCondition.JobType = null; 

			//生成查询条件
			this.BuildWhereClause();

			this.LastQueryRowCount = -1;
			this.JobDeluxeGrid.PageIndex = 0;
			this.JobDeluxeGrid.SelectedKeys.Clear();

			this.JobDeluxeGrid.DataBind();
		}

		protected JobCondition QueryCondition
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "QueryCondition", new JobCondition());
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "QueryCondition", value);
			}
		}

		/// <summary>
		/// 拼成查询条件
		/// </summary>
		private void BuildWhereClause()
		{
			WhereSqlClauseBuilder wherebuilder = ConditionMapping.GetWhereSqlClauseBuilder(this.QueryCondition);

			this.whereCondition.Value = wherebuilder.ToSqlString(TSqlBuilder.Instance);
		}

		protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		protected void hiddenServerBtn_Click(object sender, EventArgs e)
		{
			LastQueryRowCount = -1;
		}

		public string ConvertBoolToCN(bool val)
		{
			if (val)
			{
				return "是";
			}

			return "否";
		}
	}
}