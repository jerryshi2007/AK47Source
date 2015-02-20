using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Web.Controls;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.Configuration;
using MCS.Library.OGUPermission;

namespace PermissionCenter.Template
{
	public partial class SCSignInTemplate : System.Web.UI.UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.Parent is SignInControl)
			{
				InitSignInControl((SignInControl)(this.Parent));
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				TimePointContext tpc = TimePointContext.LoadFromCookie();

				if (tpc.UseCurrentTime == false)
					this.simulateDate.Value = tpc.SimulatedTime;
			}
		}

		private void InitSignInControl(SignInControl signInControl)
		{
			signInControl.AfterSignIn += new AfterSignInDelegate(signInControl_AfterSignIn);
		}

		private void signInControl_AfterSignIn(SignInContext context)
		{
			if (PassportSignInSettings.GetConfig().UseSimulateTime)
			{
				context.SignInInfo.Properties["SimulateTime"] = this.simulateDate.Value;

				IPersistTimePoint persister = TimePointSimulationSettings.GetConfig().Persister;

				if (persister != null)
				{
					OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, context.SignInInfo.UserID);

					if (users.Count > 0)
					{
						persister.SaveTimePoint(users[0].ID, this.simulateDate.Value);

						TimePointContext tpc = new TimePointContext();

						tpc.UseCurrentTime = this.simulateDate.Value == DateTime.MinValue;
						tpc.SimulatedTime = this.simulateDate.Value;

						tpc.SaveToCookie();
					}
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.simulateDateRow.Visible = PassportSignInSettings.GetConfig().UseSimulateTime;
			Page.ClientScript.RegisterHiddenField("buttonName", this.SignInButton.ClientID);

			base.OnPreRender(e);
		}
	}
}