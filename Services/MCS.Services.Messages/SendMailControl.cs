using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Utilities;
using MCS.Library.Services;

namespace MCS.Services.Messages
{
	public partial class SendMailControl : UserControl, IFunctionTestControl
	{
		private const string StorageFileName = "SendMailControl";

		public SendMailControl()
		{
			InitializeComponent();

			comboBoxAuthenticateType.DataSource = EnumItemDescriptionAttribute.GetDescriptionList(typeof(AuthenticateType));

			comboBoxAuthenticateType.ValueMember = "Name";
			comboBoxAuthenticateType.DisplayMember = "Description";

			SendMailParameters smp = (SendMailParameters)ApplicationStorage.LoadObject(StorageFileName);

			if (smp == null)
				smp = InitSMPFromEmailSettings();

			RenderControlsBySmtpParameters(smp);
		}

		private void RenderControlsBySmtpParameters(SendMailParameters smp)
		{
			SmtpParameters sp = smp.SmtpParams;

			if (sp != null)
			{
				if (sp.ServerInfo != null)
				{
					textBoxServer.Text = sp.ServerInfo.ServerName;
					textBoxPort.Text = sp.ServerInfo.Port.ToString();
					comboBoxAuthenticateType.SelectedValue = sp.ServerInfo.AuthenticateType.ToString();

					if (sp.ServerInfo.Identity != null)
					{
						textBoxLogOnName.Text = sp.ServerInfo.Identity.LogOnName;
						textBoxPassword.Text = sp.ServerInfo.Identity.Password;
					}
				}

				if (sp.DefaultSender != null)
					textBoxSignInAddress.Text = sp.DefaultSender.ToString();

				textBoxMessage.Text = smp.DefaultEmailSubject;
			}

			textBoxMessage.Text = smp.DefaultEmailSubject;

			if (smp.ToAddress != null)
				textBoxDest.Text = smp.ToAddress.ToString();

			SetAuthGroupBoxEnabled();
		}

		private void buttonSend_Click(object sender, System.EventArgs e)
		{
			try
			{
				ExceptionHelper.CheckStringIsNullOrEmpty(textBoxDest.Text, "收件人地址");

				SendMailParameters smp = CollectInfo();

				EmailMessage message = new EmailMessage(smp.ToAddress, smp.DefaultEmailSubject, string.Empty);

				message.From = smp.SmtpParams.DefaultSender;

				EmailMessageAdapter.Instance.Send(message, smp.SmtpParams);

				ShowMessage("发送完成");
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void SetAuthGroupBoxEnabled()
		{
			AuthenticateType authType = (AuthenticateType)Enum.Parse(typeof(AuthenticateType), (string)comboBoxAuthenticateType.SelectedValue);

			groupBoxAuth.Enabled = (authType == AuthenticateType.Basic);
		}

		private SendMailParameters CollectInfo()
		{
			SendMailParameters result = new SendMailParameters();

			SmtpParameters sp = new SmtpParameters();

			LogOnIdentity identity = new LogOnIdentity(textBoxLogOnName.Text, textBoxPassword.Text);

			ServerInfo serverInfo = new ServerInfo(textBoxServer.Text, identity);

			int port = 0;

			if (int.TryParse(textBoxPort.Text, out port))
				serverInfo.Port = port;

			serverInfo.AuthenticateType = (AuthenticateType)Enum.Parse(typeof(AuthenticateType), (string)comboBoxAuthenticateType.SelectedValue);

			sp.ServerInfo = serverInfo;
			sp.UseDefaultCredentials = serverInfo.AuthenticateType == AuthenticateType.Anonymous;

			if (textBoxSignInAddress.Text.IsNotEmpty())
				sp.DefaultSender = EmailAddress.FromDescription(textBoxSignInAddress.Text);

			sp.AfterSentOP = EmailMessageAfterSentOP.NotPersisted;

			result.SmtpParams = sp;
			result.DefaultEmailSubject = textBoxMessage.Text;
			result.ToAddress = EmailAddress.FromDescription(textBoxDest.Text);

			return result;
		}

		private void comboBoxAuthenticateType_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			SetAuthGroupBoxEnabled();
		}

		private SendMailParameters InitSMPFromEmailSettings()
		{
			SendMailParameters smp = new SendMailParameters();

			smp.DefaultEmailSubject = textBoxMessage.Text;

			try
			{
				smp.SmtpParams = EmailMessageSettings.GetConfig().ToSmtpParameters();
			}
			catch (System.Configuration.ConfigurationException)
			{
			}

			return smp;
		}

		public void OnClosing(EventArgs e)
		{
			SendMailParameters smp = this.CollectInfo();

			ApplicationStorage.SaveObject(StorageFileName, smp);
		}

		private void buttonResetFromConfig_Click(object sender, EventArgs e)
		{
			try
			{
				SendMailParameters smp = InitSMPFromEmailSettings();
				RenderControlsBySmtpParameters(smp);

				ShowMessage("重置完成");
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void buttonSendOneCandidate_Click(object sender, EventArgs e)
		{
			try
			{
				SendMailParameters smp = CollectInfo();
				//EmailMessageAdapter.Instance.SendCandidateMessages(1, smp.SmtpParams);
				EmailMessage message = new EmailMessage(smp.ToAddress, smp.DefaultEmailSubject, string.Empty);

				message.From = smp.SmtpParams.DefaultSender;

				EmailMessageAdapter.Instance.Insert(message);

				ShowMessage("发送完成");
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void ShowMessage(string text)
		{
			MessageBox.Show(this, text, "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
