using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;

namespace MCSPendingActService
{
	public partial class PendingActServiceControl : UserControl
	{
		public PendingActServiceControl()
		{
			InitializeComponent();
		}

		private void btnView_Click(object sender, EventArgs e)
		{
			WfPendingActivityInfoCollection pendingActs;

			pendingActs = WfPendingActivityInfoAdapter.Instance.LoadAll();

			txtActivityID.Text = "";
			foreach (var pendingAct in pendingActs)
			{
				txtActivityID.Text += pendingAct.ActivityID + "/" + pendingAct.ProcessID + Environment.NewLine;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var pendingActList = WfPendingActivityInfoAdapter.Instance.Load(this.txtAppName.Text.Trim(), this.txtProgramName.Text.Trim());

			foreach (var item in pendingActList)
			{
				WfRuntime.ProcessPendingActivity(item);
			}
		}

		private void btnSingleActivity_Click(object sender, EventArgs e)
		{
			var pendingActList = WfPendingActivityInfoAdapter.Instance.Load(p => p.AppendItem("ACTIVITY_ID", this.txtActID.Text.Trim()));
			if (pendingActList.Count == 0)
			{
				MessageBox.Show("没找到相应的数据");
				return;
			}
			
			WfRuntime.ProcessPendingActivity(pendingActList.First());
		}
	}
}
