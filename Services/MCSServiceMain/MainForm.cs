using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using MCS.Library.Logging;
using MCS.Library.Configuration;

namespace MCS.Library.Services
{
	public partial class MainForm : Form
	{
		private class TabPageTag
		{
			private Control innerControl;
			private TypeConfigurationElement controlDesp;

			public TabPageTag(TypeConfigurationElement desp)
			{
				this.controlDesp = desp;
			}

			public TypeConfigurationElement ControlDesp
			{
				get
				{
					return this.controlDesp;
				}
			}

			public Control InnerControl
			{
				get
				{
					return this.innerControl;
				}
				set
				{
					this.innerControl = value;
				}
			}
		}

		private MCSServiceMain serviceMain;

		public MainForm(MCSServiceMain serviceMain)
		{
			InitializeComponent();

			this.serviceMain = serviceMain;

			CreateAddIns();

			this.serviceMain.Log.AddTextBoxTraceListener(this.textBoxLog);
			this.serviceMain.CreateThreadEvent += new CreateThreadDelegete(OnCreateThreadEvent);

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
		}

		private void CreateAddIns()
		{
			TypeConfigurationCollection addIns = ServiceMainSettings.GetConfig().AddIns;

			foreach (TypeConfigurationElement ele in addIns)
			{
				TabPage page = new TabPage(ele.Description);

				page.Tag = new TabPageTag(ele);

				this.tabControlClient.TabPages.Add(page);
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			//this.outputBuffer.Remove(0, this.outputBuffer.Length);
			this.textBoxLog.Text = string.Empty;

			this.serviceMain.StarService();

			this.btnStop.Enabled = true;
			this.btnStart.Enabled = false;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			this.serviceMain.StopService();

			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.serviceMain.StopService();

			CloseAllInnerControls(e);
		}

		private void CloseAllInnerControls(FormClosingEventArgs e)
		{
			foreach (TabPage tabPage in this.tabControlClient.TabPages)
			{
				if (tabPage.Tag is TabPageTag)
				{
					TabPageTag tpt = (TabPageTag)tabPage.Tag;

					if (tpt.InnerControl != null && tpt.InnerControl is IFunctionTestControl)
						((IFunctionTestControl)tpt.InnerControl).OnClosing(e);
				}
			}
		}

		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show(this, e.Exception.Message + "\n" + e.Exception.StackTrace, "´íÎó", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}

		private void OnCreateThreadEvent(ThreadParam tp)
		{
			tp.Log.AddTextBoxTraceListener(this.textBoxLog);
		}

		private void tabControlClient_SelectedIndexChanged(object sender, EventArgs e)
		{
			TabPageTag tpt = (TabPageTag)tabControlClient.SelectedTab.Tag;

			if (tpt != null)
			{
				if (tpt.InnerControl == null)
				{
					try
					{
						IFunctionTestControlCreator creator = (IFunctionTestControlCreator)tpt.ControlDesp.CreateInstance();

						Control ctrl = creator.CreateControl(serviceMain.args);

						ctrl.Parent = tabControlClient.SelectedTab;
						ctrl.Dock = DockStyle.Fill;

						tpt.InnerControl = ctrl;
					}
					catch (Exception ex)
					{
						this.serviceMain.Log.Write(ex, ServiceLogEventID.SERVICEMAIN_CREATECONTROL);
					}
				}
			}
		}
	}
}