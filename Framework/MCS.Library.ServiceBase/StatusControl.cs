using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Core;
using System.Runtime.Remoting.Channels.Http;

namespace MCS.Library.Services
{
	public partial class StatusControl : UserControl
	{
		private IStatusService statusService = null;
		private bool missingLogged = false;

		public StatusControl()
		{
			InitializeComponent();

			if (IsDesignMode == false)
			{
				InitListViewColumns();

				timerStatus.Enabled = true;
			}
		}

		private void InitListViewColumns()
		{
			listViewThreadStatus.Columns.Add("�����߳�����", 128, HorizontalAlignment.Left);
			listViewThreadStatus.Columns.Add("ִ��״̬", 128, HorizontalAlignment.Left);
			listViewThreadStatus.Columns.Add("�����ѯʱ��", 128, HorizontalAlignment.Left);
			listViewThreadStatus.Columns.Add("������Ϣ", 128, HorizontalAlignment.Left);
			listViewThreadStatus.Columns.Add("�����쳣", 128, HorizontalAlignment.Left);
		}

		private bool InitStatusService()
		{
			this.statusService = null;

			try
			{
				//string uri = ServiceStatusSettings.GetConfig().Servers["defaultService"].Description;

				string uri;

				if (ServiceStatusSettings.GetConfig().Servers["defaultService"] == null)
				{
					if (missingLogged == false)
					{
						string failMessage = "δ���ҵ���Ϊ defaultService ��״̬�������ã���˿����޷��鿴״̬����";
						EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, failMessage, EventLogEntryType.Warning);
						if (this.InvokeRequired)
							this.Invoke(new Action<string>(this.ShowMessageBox), failMessage);
						else
							this.ShowMessageBox(failMessage);

						missingLogged = true;
					}
				}
				else
				{
					uri = ServiceStatusSettings.GetConfig().Servers["defaultService"].Description;

					try
					{
						//���Ի�ȡ��ǰӦ��ע��ķ����ַ
						uri = GetServiceStatusUri();
					}
					catch (Exception ex)
					{
						EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, ex.Message, EventLogEntryType.Warning);
					}

					this.statusService = (IStatusService)Activator.GetObject(typeof(IStatusService), uri);
				}
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, ex.Message, EventLogEntryType.Warning);
			}

			return this.statusService != null;
		}

		private void ShowMessageBox(string message)
		{
			MessageBox.Show(this, message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private static string GetServiceStatusUri()
		{
			IChannel[] channels = ChannelServices.RegisteredChannels;

			(channels.Length > 0).FalseThrow("û��ע��Channel");

			WellKnownServiceTypeEntry[] entries = RemotingConfiguration.GetRegisteredWellKnownServiceTypes();

			(entries.Length > 0).FalseThrow("û��ע���������");

			HttpChannel channel = channels[0] as HttpChannel;

			(channel != null).FalseThrow("Channel������HttpChannel");

			ChannelDataStore dataStore = (ChannelDataStore)channel.ChannelData;

			(dataStore.ChannelUris.Length > 0).FalseThrow("Channel��ChannelUris�ĸ����������0");

			return dataStore.ChannelUris[0] + "/" + entries[0].ObjectUri;
		}

		private void timerStatus_Tick(object sender, EventArgs e)
		{
			queryStatusWorker.RunWorkerAsync();
			timerStatus.Enabled = false;
		}

		private static bool IsDesignMode
		{
			get
			{
				string fileName = Path.GetFileName(Application.ExecutablePath);
				return string.Compare(fileName, "devenv.exe") == 0;
			}
		}

		private void queryStatusWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				if (InitStatusService())
				{
					BackgroundWorker worker = (BackgroundWorker)sender;
					e.Result = this.statusService.GetThreadStatus();
				}
			}
			catch (Exception ex)
			{
				e.Result = ex.Message;
			}
		}

		private void queryStatusWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			timerStatus.Enabled = true;

			if (e.Result is ServiceThreadCollection)
			{
				listViewThreadStatus.BeginUpdate();

				try
				{
					ServiceThreadCollection status = (ServiceThreadCollection)e.Result;

					listViewThreadStatus.Items.Clear();

					foreach (IServiceThread thread in status)
					{
						ListViewItem item = new ListViewItem();

						item.Text = thread.Params.Name;
						item.SubItems.Add(thread.Status.ToString());

						if (thread.LastPollTime != DateTime.MinValue)
							item.SubItems.Add(thread.LastPollTime.ToString("yyyy-MM-dd HH:mm:ss"));
						else
							item.SubItems.Add(string.Empty);

						item.SubItems.Add(thread.LastMessage);
						item.SubItems.Add(thread.LastExceptionMessage);

						listViewThreadStatus.Items.Add(item);
					}
				}
				finally
				{
					listViewThreadStatus.EndUpdate();
				}
			}
		}
	}
}
