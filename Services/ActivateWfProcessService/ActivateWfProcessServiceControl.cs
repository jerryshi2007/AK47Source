using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;

namespace ActivateWfProcessService
{
	public partial class ActivateWfProcessServiceControl : UserControl
	{
		private JobCollection WfJobs { get; set; }

		public ActivateWfProcessServiceControl()
		{
			InitializeComponent();

			BindTaskList();
		}

		private void BindTaskList()
		{
			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();
			whereBuilder.AppendItem("ENABLED", "1");
			whereBuilder.AppendItem("JOB_STATUS", 0);
			string category = txtCategory.Text.Trim();
			string name = txtName.Text.Trim();
			if (string.IsNullOrEmpty(category) == false)
				whereBuilder.AppendItem("JOB_CATEGORY", category);
			if (string.IsNullOrEmpty(name) == false)
				whereBuilder.AppendItem("JOB_NAME", name);

			this.WfJobs = JobBaseAdapter.Instance.UPDLOCKLoadJobs(decimal.ToInt32(numericUpDown1.Value), whereBuilder);

			this.dataGridView1.DataSource = this.WfJobs;
		}

		private void btnRun_Click(object sender, EventArgs e)
		{
			var timeOffset = new TimeSpan(0, 0, 10);

			StringBuilder logger = new StringBuilder();
			foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
			{
				var id = row.Cells[0].Value.ToString();
				var job = this.WfJobs[id];

				WhereSqlClauseBuilder singleWhere = new WhereSqlClauseBuilder();
				singleWhere.AppendItem("JOB_ID", job.JobID);
				if (job.JobType == JobType.StartWorkflow)
					job = StartWorkflowJobAdapter.Instance.LoadSingleData(singleWhere);
				//StartWorkflowJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", job.JobID)).Single();
				else
					job = InvokeWebServiceJobAdapter.Instance.LoadSingleData(singleWhere);
				///job = InvokeWebServiceJobAdapter.Instance.Load(p => p.AppendItem("JOB_ID", job.JobID)).Single();

				try
				{
					job.SetCurrentJobBeginStatus();
					job.Start();
					logger.AppendFormat("任务{0}执行完成！", job.Name);
				}
				catch (Exception ex)
				{
					logger.AppendFormat("任务{0}发生错误，{1}{2}", job.Name, ex.Message, ex.StackTrace);
				}
				finally
				{
					job.SetCurrentJobEndStatus();
				}
			}

			BindTaskList();
			this.textBox1.Text = logger.ToString();
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			BindTaskList();
		}
	}
}
