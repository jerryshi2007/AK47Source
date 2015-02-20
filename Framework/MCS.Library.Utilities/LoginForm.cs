using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Core;

namespace MCS.Library.Utilities
{
	public partial class LoginForm : Form
	{
		private LogOnIdentity identity = null;

		public LogOnIdentity Identity
		{
			get { return identity; }
			set { identity = value; }
		}

		public LoginForm()
		{
			InitializeComponent();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.identity = new LogOnIdentity(this.textBoxUser.Text, this.textBoxPassword.Text, this.textBoxDC.Text);
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			if (this.identity != null)
			{
				this.textBoxUser.Text = this.identity.LogOnName;
				this.textBoxPassword.Text = this.identity.Password;
				this.textBoxDC.Text = this.identity.Domain;
			}
		}		
	}
}