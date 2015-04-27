using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Web.Controls;
using MCS.Web.Responsive.Library;
using MCS.Library.Configuration;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.Globalization;

namespace ResponsivePassportService.Template
{
    public partial class ResponsiveSignInTemplate : System.Web.UI.UserControl
    {
        private SignInControl _Owner = null;

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

        protected override void OnPreRender(EventArgs e)
        {
            HtmlAnchor anchor = new HtmlAnchor();

            anchor.ID = "bindingUrlTemplate";
            anchor.HRef = GetBindingPageUrl();

            this.bindingUrlContainer.Text = anchor.GetHtml();

            this.simulateDateRow.Visible = PassportSignInSettings.GetConfig().UseSimulateTime;

            if (this._Owner != null)
            {
                this.culture.Value = this._Owner.PageData.Properties.GetValue("SignInCulture", "zh-CN");

                string tenantCode = PassportManager.GetTenantCodeFromContext();

                if (tenantCode.IsNullOrEmpty())
                    tenantCode = this._Owner.PageData.Properties.GetValue("TenantCode", string.Empty);

                this.tenantCode.Text = tenantCode;
            }

            base.OnPreRender(e);
        }

        protected static string GetBindingPageUrl()
        {
            NameValueCollection paremeters = HttpContext.Current.Request.Url.GetUriParamsCollection();

            return UriHelper.CombineUrlParams("Bind.aspx", paremeters);
        }

        private void InitSignInControl(SignInControl signInControl)
        {
            signInControl.AfterSignIn += new AfterSignInDelegate(signInControl_AfterSignIn);
            signInControl.BeforeAuthenticate += signInControl_BeforeAuthenticate;
            this._Owner = signInControl;
        }

        private void signInControl_BeforeAuthenticate(LogOnIdentity loi)
        {
            TenantContext.Current.TenantCode = this.tenantCode.Text;
        }

        private void signInControl_AfterSignIn(SignInContext context)
        {
            if (context.Exception == null)
            {
                if (PassportSignInSettings.GetConfig().UseSimulateTime)
                    SaveSimulateTime(context, this.simulateDate.Value);

                SetCultureInfo(context, this.culture.Value);

                TenantContext.Current.TenantCode = this.tenantCode.Text;
                context.SignInInfo.TenantCode = this.tenantCode.Text;

                context.PageData.Properties["SignInCulture"] = this.culture.Value;
                context.PageData.Properties["TenantCode"] = this.tenantCode.Text;
            }
        }

        private static void SaveSimulateTime(SignInContext context, DateTime simulateDate)
        {
            context.SignInInfo.Properties["SimulateTime"] = simulateDate;

            IPersistTimePoint persister = TimePointSimulationSettings.GetConfig().Persister;

            if (persister != null)
            {
                OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, context.SignInInfo.UserID);

                if (users.Count > 0)
                {
                    persister.SaveTimePoint(users[0].ID, simulateDate);

                    TimePointContext tpc = new TimePointContext();

                    tpc.UseCurrentTime = simulateDate == DateTime.MinValue;
                    tpc.SimulatedTime = simulateDate;

                    tpc.SaveToCookie();
                }
            }
        }

        private static void SetCultureInfo(SignInContext context, string language)
        {
            context.SignInInfo.Properties["Culture"] = language;

            IUserCultureInfoAccessor accessor = UserCultureInfoSettings.GetConfig().UserCultureInfoAccessor;

            if (accessor != null)
                accessor.SaveUserLanguageID(HttpContext.Current.User.Identity.Name, language);
        }
    }
}