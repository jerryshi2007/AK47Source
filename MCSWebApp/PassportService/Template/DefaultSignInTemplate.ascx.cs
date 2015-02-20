using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.Passport;
using MCS.Library.Web.Controls;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Web.Passport.Template
{
    public partial class DefaultSignInTemplate : System.Web.UI.UserControl
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
			if (context.ResultType == SignInResultType.Success)
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
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.simulateDateRow.Visible = PassportSignInSettings.GetConfig().UseSimulateTime;

            base.OnPreRender(e);
        }
    }
}