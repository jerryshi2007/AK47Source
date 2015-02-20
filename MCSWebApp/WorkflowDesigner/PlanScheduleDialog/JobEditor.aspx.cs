using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Web;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.Core;
using System.Transactions;
using MCS.Library.Data;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public partial class JobEditor : System.Web.UI.Page
	{
		private string JobID { get; set; }
		private PageMode Mode { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize();
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		private void Initialize()
		{
			WebUtility.RequiredScript(typeof(ControlBaseScript));
			this.JobID = WebUtility.GetRequestQueryValue("jobId", string.Empty);
			if (string.IsNullOrEmpty(this.JobID))
			{
				this.JobID = Guid.NewGuid().ToString();
				this.Mode = PageMode.Create;
			}
			else
			{
				this.Mode = PageMode.Edit;
			}

			if (!IsPostBack)
			{
				var jobTypeDesc = EnumItemDescriptionAttribute.GetDescriptionList(typeof(JobType));

				this.ddlType.DataTextField = "Description";
				this.ddlType.DataValueField = "EnumValue";
				this.ddlType.DataSource = jobTypeDesc;
				this.ddlType.DataBind();

				this.ddlProcess.DataSource = WfProcessDescHelper.GetAllProcessDescKeys(true);
				this.ddlProcess.DataBind();

				if (this.Mode == PageMode.Create)
				{
					detailGrid.InitialData = new List<SimpleSchedule>();
				}
				else
				{
					InitControlState();
				}
			}
		}

		private void InitControlState()
		{
			var jobType = JobBaseAdapter.Instance.GetJobType(this.JobID);

			if (jobType == JobType.StartWorkflow)
			{
				var coll = StartWorkflowJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", this.JobID));
				if (coll.Count == 0)
				{
					throw new Exception("无法找到ID为" + this.JobID + "的任务");
				}

				var job = coll[0];
				SetJobBaseControlState(job);
				OuUserInputControl.SelectedSingleData = job.Operator;
				if (ddlProcess.Items.FindByValue(job.ProcessKey) == null)
				{
					ListItem item = new ListItem() { Value = job.ProcessKey, Selected = true };
					WfProcessDescriptorInfoCollection processDescInfos =
						WfProcessDescriptorInfoAdapter.Instance.LoadWfProcessDescriptionInfos(builder => builder.AppendItem("PROCESS_KEY", job.ProcessKey), true);

					if (processDescInfos.Count > 0)
					{
						item.Text = job.ProcessKey + "-" + processDescInfos[0].ProcessName;
					}
					ddlProcess.Items.Add(item);
				}
				else
				{
					ddlProcess.Items.FindByValue(job.ProcessKey).Selected = true;
				}
			}
			else if (jobType == JobType.InvokeService)
			{
				var coll = InvokeWebServiceJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", this.JobID));

				(coll.Count > 0).FalseThrow<ArgumentException>("无法找到ID为{0}的任务", this.JobID);

				var job = coll[0];
				SetJobBaseControlState(job);
				this.invokingServiceGrid.InitialData = job.SvcOperationDefs;
			}
		}

		private void SetJobBaseControlState(JobBase job)
		{
			txtName.Value = job.Name;
			txtDesc.Value = job.Description;
			ddlEnabled.Items.FindByValue(job.Enabled.ToString().ToLower()).Selected = true;
			ddlType.SelectedValue = ((int)job.JobType).ToString();
			txtCategory.Value = job.Category;

			var simpleSchedules = from item in job.Schedules
								  select new SimpleSchedule()
								  {
									  ID = item.ID,
									  Name = item.Name,
									  Description = item.Description,
									  Enabled = item.Enabled,
									  StartTime = item.StartTime,
									  EndTime = item.EndTime
								  };

			detailGrid.InitialData = simpleSchedules.ToList();
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			Initialize();
		}

		#region save
		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			JobType jobType = (JobType)Enum.Parse(typeof(JobType), this.ddlType.SelectedValue);

			JobBase newJob = null;
			switch (jobType)
			{
				case JobType.StartWorkflow:
					newJob = CreateStartWorkflowJob();
					break;
				case JobType.InvokeService:
					newJob = CreateInvokingServiceJob();
					break;
			}

			if (newJob == null)
			{
				WebUtility.ShowClientError("不被支持的任务类型！", "", "错误");
			}

			try
			{
				DoUpdate(newJob);
				Page.ClientScript.RegisterStartupScript(this.GetType(), "updateJob",
					string.Format("window.returnValue = true; top.close();"), true);

			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}

		private void DoUpdate(JobBase newJob)
		{
			using (TransactionScope ts = TransactionScopeFactory.Create())
			{
				if (this.Mode == PageMode.Edit)
				{
					StartWorkflowJobAdapter.Instance.Delete(new string[] { newJob.JobID });
					InvokeWebServiceJobAdapter.Instance.Delete(p => p.AppendItem("JOB_ID", newJob.JobID));
				}
				if (newJob is StartWorkflowJob)
				{
					StartWorkflowJobAdapter.Instance.Update((StartWorkflowJob)newJob);
				}
				else if (newJob is InvokeWebServiceJob)
				{
					InvokeWebServiceJobAdapter.Instance.Update((InvokeWebServiceJob)newJob);
				}
				else
				{
					JobBaseAdapter.Instance.Update(newJob);
				}

				ts.Complete();
			}
		}

		private JobBase CreateStartWorkflowJob()
		{
			StartWorkflowJob newJob = new StartWorkflowJob();
			SetJobBaseInfo(newJob);
			newJob.ProcessKey = this.ddlProcess.SelectedValue;
			newJob.Operator = (IUser)OuUserInputControl.SelectedSingleData;
			return newJob;
		}

		private JobBase CreateInvokingServiceJob()
		{
			WfConverterHelper.RegisterConverters();

			InvokeWebServiceJob newJob = new InvokeWebServiceJob();
			SetJobBaseInfo(newJob);
			newJob.SvcOperationDefs = JSONSerializerExecute.Deserialize<WfServiceOperationDefinitionCollection>(this.hdServiceDefinition.Value);

			return newJob;
		}

		private void SetJobBaseInfo(JobBase newJob)
		{
			newJob.JobID = this.JobID;
			newJob.Name = txtName.Value.Trim();
			newJob.Enabled = bool.Parse(this.ddlEnabled.Value);
			newJob.Description = txtDesc.Value;
			newJob.Creator = DeluxeIdentity.CurrentUser;
			newJob.Category = txtCategory.Value;

			JobScheduleCollection schedules = new JobScheduleCollection();
			foreach (SimpleSchedule item in detailGrid.InitialData)
			{
				JobSchedule schedule = JobScheduleAdapter.Instance.Load(p => p.AppendItem("SCHEDULE_ID", item.ID))[0];
				schedules.Add(schedule);
			}
			newJob.Schedules = schedules;
		}
		#endregion
	}
}