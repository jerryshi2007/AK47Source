#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInControl.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Xml;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Passport;
using MCS.Library.Passport.Properties;
using MCS.Library.Globalization;
using MCS.Library.Passport.Performance;

namespace MCS.Library.Web.Controls
{
    /// <summary>
    /// 单点登录控件类
    /// </summary>
    [ToolboxData("<{0}:SignInControl runat=server></{0}:SignInControl>")]
    public class SignInControl : WebControl
    {
        private System.Web.UI.Control templateControl = null;
        private string templatePath = string.Empty;
        private SignInPageData _PageData = new SignInPageData();

        /// <summary>
        /// 初始化SignInControl的事件，如果提供了认证信息，则不再显示认证页面，自动转向
        /// </summary>
        public event InitSignInControlDelegate InitSignInControl;

        /// <summary>
        /// 认证之前的事件定义。可以修改待认证的用户名和口令
        /// </summary>
        public event BeforeAuthenticateDelegate BeforeAuthenticate;

        /// <summary>
        /// 认证之后的事件定义。可以修改认证之后用户登录名，也可以截获认证时产生的异常
        /// </summary>
        public event AfterSignInDelegate AfterSignIn;

        /// <summary>
        /// 页面模板的路径
        /// </summary>
        [Bindable(true),
            Category("Appearance"),
            DefaultValue("")]
        public string TemplatePath
        {
            get
            {
                return this.templatePath;
            }
            set
            {
                this.templatePath = value;
            }
        }

        /// <summary>
        /// 模板控件的实例
        /// </summary>
        [Browsable(false)]
        public Control Template
        {
            get
            {
                return this.templateControl;
            }
        }

        /// <summary>
        /// 认证页面所保存在Cookie中的信息
        /// </summary>
        public SignInPageData PageData
        {
            get
            {
                return this._PageData;
            }
        }

        /// <summary>
        /// 初始化通知器
        /// </summary>
        protected override void CreateChildControls()
        {
            //InitNotifiers();
        }

        #region 私有方法
        private void InitScript()
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SignInScriptOnload", Resource.signInControlScript);

            string template = ResourceHelper.LoadStringFromResource(
                Assembly.GetExecutingAssembly(),
                "MCS.Library.Passport.Controls.SignInControlInitScript.htm");

            string script = string.Format(template, PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "ClientID", string.Empty),
                PassportWebControlHelper.GetControlValue(this.TemplateControl, "password", "ClientID", string.Empty));

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "SignInScriptOnload",
                script, true);

            Control detailErrorMessageLink = PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "detailErrorMessageLink");

            if (detailErrorMessageLink != null && detailErrorMessageLink is HtmlContainerControl)
                ((HtmlContainerControl)detailErrorMessageLink).Attributes["onclick"] =
                    string.Format("return doDetailErrorMessageClick(\"{0}\");",
                    PassportWebControlHelper.GetControlValue(this.TemplateControl, "detailErrorMessage", "ClientID", string.Empty));
        }

        /// <summary>
        /// 进行初始化，加载TemplatePath页面，JavaScript脚本，进行用户SignInInfo状态判断
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (this.DesignMode == false)
            {
                ISignInUserInfo signInUserInfo = OnInitSignInControl();

                if (signInUserInfo != null)
                {
                    ITicket ticket = Ticket.Create(SignInInfo.Create(signInUserInfo,
                        (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "dontSaveUserName", "Checked", false),
                        (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "autoSignIn", "Checked", false)),
                        HttpContext.Current.Request.QueryString["ip"]);

                    RedirectToAppUrl(ticket);
                }
                else
                {
                    if (TemplatePath != string.Empty)
                    {
                        this.templateControl = Page.LoadControl(TemplatePath);

                        this.Controls.Add(this.templateControl);

                        InitScript();
                        Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// 渲染控件
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write(string.Format("<img src=\"{0}\"/>",
                    Page.ClientScript.GetWebResourceUrl(this.GetType(),
                    Translator.Translate(Common.CultureCategory, "MCS.Library.Passport.Resources.signin.gif"))));
            else
                base.Render(writer);
        }

        /// <summary>
        /// 判断SignInInfo状态，如果在SignInInfo合法情况下，基于Session的Cookie，或者用户选择自动登录的进行自动登录。
        /// </summary>
        private void Initialize()
        {
            Control signInBtn = PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "SignInButton");

            if (signInBtn != null && signInBtn is IButtonControl)
                ((IButtonControl)signInBtn).Click += new EventHandler(SignInButton_Click);

            if (Page.IsPostBack == false)
            {
                //从Cookie中得到登录信息
                ISignInInfo signInInfo = SignInInfo.LoadFromCookie();

                if (signInInfo != null)
                    Trace.WriteLine(string.Format("认证服务，从cookie中得到用户{0}的认证信息", signInInfo.UserID), "PassportSDK");

                this._PageData.LoadFromCookie();

                PassportSignInSettings settings = PassportSignInSettings.GetConfig();

                if (IsSignInInfoInvalid(signInInfo) == false)	//SignIn Info非法
                {
                    if (settings.IsSessionBased || this.PageData.AutoSignIn)
                        if (IsSelfAuthenticate == false)
                            AutoSignIn(signInInfo);	//May be execute Response.End when redirect to app's url
                }

                if (Page.IsPostBack == false)
                    InitForm(this.PageData);

                Page.Response.Expires = 0;
            }
        }

        private void InitForm(SignInPageData pd)
        {
            if (pd.DontSaveUserID == false)
                PassportWebControlHelper.SetControlValue(this.TemplateControl, "signInName", "Text", pd.UserID);

            PassportWebControlHelper.SetControlValue(this.TemplateControl, "autoSignIn", "Checked", pd.AutoSignIn);
            PassportWebControlHelper.SetControlValue(this.TemplateControl, "dontSaveUserName", "Checked", pd.DontSaveUserID);
            PassportWebControlHelper.SetControlValue(this.TemplateControl, "autoSignIn", "Visible", PassportSignInSettings.GetConfig().IsSessionBased == false);
        }

        /// <summary>
        /// 如果登录信息依然有效，可以自动转到应用页面
        /// </summary>
        /// <param name="signInInfo">登录信息</param>
        private void AutoSignIn(ISignInInfo signInInfo)
        {
#if DELUXEWORKSTEST
			Debug.WriteLine("Timeout Datetime: " + signInInfo.SignInTimeout.ToString("yyyy-MM-dd HH:mm:ss"), "SignInPage Check");
#endif
            if (IsSignInInfoInvalid(signInInfo) == false)
            {
                AdjustSignInTimeout(signInInfo);

                PassportSignInSettings.GetConfig().PersistSignInInfo.SaveSignInInfo(signInInfo);

                signInInfo.SaveToCookie();
                RedirectToAppUrl(GenerateTicket(signInInfo));
            }
        }

        private void AdjustSignInTimeout(ISignInInfo signInInfo)
        {
            if (PassportSignInSettings.GetConfig().HasSlidingExpiration)
                signInInfo.SignInTime = DateTime.Now;
        }

        /// <summary>
        /// 是否是自认证页面，此类页面没有ru参数
        /// </summary>
        /// <returns>是否是自认证页面</returns>
        private bool IsSelfAuthenticate
        {
            get
            {
                string strReturnUrl = HttpContext.Current.Request.QueryString["ru"];

                return strReturnUrl == null;
            }
        }

        private bool IsSignInInfoInvalid(ISignInInfo signInInfo)
        {
            bool invalid = true;

            if (signInInfo != null)
                invalid = signInInfo.IsValid() == false;

            return invalid;
        }

        #region Event Handler
        private void SignInButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ITicket ticket = AuthenticateUser();

                RedirectToAppUrl(ticket);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (System.Exception ex)
            {
                System.Exception realEx = ex.GetRealException();

                string strSignInName = (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "Text", string.Empty);

                SignInContext signInContext = new SignInContext(
                    SignInResultType.Fail,
                    strSignInName,
                    null,
                    CollectPageData(),
                    (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "clientEnv", "Value", string.Empty),
                    realEx);

                OnSignInComplete(signInContext);

                PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "Text", HttpUtility.HtmlEncode(realEx.Message));
                PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "ForeColor", Color.Red);
                PassportWebControlHelper.SetControlValue(this.TemplateControl, "detailErrorMessage", "Text", HttpUtility.HtmlEncode(ex.StackTrace));

                Control detailErrorMessageLink = PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "detailErrorMessageLink");

                if (detailErrorMessageLink != null && detailErrorMessageLink is HtmlContainerControl)
                    ((HtmlContainerControl)detailErrorMessageLink).Style["display"] = "block";
            }
        }
        #endregion

        private void RedirectToAppUrl(ITicket ticket)
        {
            PassportManager.SignInServiceRedirectToApp(ticket);

            if (ticket.SignInInfo != null && Page.IsPostBack)
            {
                PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "ForeColor", Color.Green);
                PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "Text", "已经登录成功");
            }
        }

        private ITicket AuthenticateUser()
        {
            HttpRequest request = HttpContext.Current.Request;

            string strSignInName = (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "Text", string.Empty);
            string strPassword = (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "password", "value", string.Empty);

            ISignInUserInfo userInfo = DefaultAuthenticate(strSignInName, strPassword);

            ISignInInfo signInInfo = SignInInfo.Create(userInfo,
                (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "dontSaveUserName", "Checked", false),
                (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "autoSignIn", "Checked", false));

            SignInContext signInContext = DoPostAuthenticateOP(signInInfo);

            ITicket ticket = Ticket.Create(signInInfo, request.QueryString["ip"]);

            SaveFormStatus(signInContext.PageData);

            return ticket;
        }

        private SignInPageData CollectPageData()
        {
            string strSignInName = (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "Text", string.Empty);

            this.PageData.UserID = strSignInName;
            this.PageData.DontSaveUserID = (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "dontSaveUserName", "Checked", false);
            this.PageData.AutoSignIn = (bool)PassportWebControlHelper.GetControlValue(this.TemplateControl, "autoSignIn", "Checked", false);

            return this.PageData;
        }

        private void SaveFormStatus(SignInPageData pageData)
        {
            pageData.SaveToCookie();
        }

        private SignInContext DoPostAuthenticateOP(ISignInInfo signInInfo)
        {
            SignInContext result = new SignInContext(
                                            SignInResultType.Success,
                                            signInInfo.UserID, signInInfo,
                                            CollectPageData(),
                                            (string)PassportWebControlHelper.GetControlValue(this.TemplateControl, "clientEnv", "Value", string.Empty),
                                            null);
            OnSignInComplete(result);

            PassportSignInSettings.GetConfig().PersistSignInInfo.SaveSignInInfo(signInInfo);

            signInInfo.SaveToCookie();

            return result;
        }

        private void OnSignInComplete(SignInContext result)
        {
            if (AfterSignIn != null)
                AfterSignIn(result);
        }

        private ISignInUserInfo OnInitSignInControl()
        {
            ISignInUserInfo result = null;

            if (InitSignInControl != null)
            {
                SignInPerformanceCounters.DoAction(() => result = InitSignInControl());
            }

            return result;
        }

        private void OnBeforeAuthenticate(LogOnIdentity loi)
        {
            if (BeforeAuthenticate != null)
                BeforeAuthenticate(loi);
        }

        private string GetViewStateString(string strKey)
        {
            return (string)PassportWebControlHelper.GetViewState(this.ViewState, strKey, string.Empty);
        }
        #endregion

        #region Ticket系列
        private ITicket GenerateTicket(ISignInInfo signInInfo)
        {
            HttpRequest request = HttpContext.Current.Request;

            string strIP = request.QueryString["ip"];

            if (strIP == null || strIP == string.Empty)
                strIP = request.UserHostAddress;

            return new Ticket(Common.GenerateTicketString(signInInfo, strIP));
        }
        #endregion

        #region Windows SignIn
        /// <summary>
        /// 调用认证接口
        /// </summary>
        /// <param name="strSignInName">登录名</param>
        /// <param name="strPasspord">密码</param>
        /// <returns>用户认证的结果</returns>
        private ISignInUserInfo DefaultAuthenticate(string strSignInName, string strPasspord)
        {
            LogOnIdentity loi = new LogOnIdentity(strSignInName, strPasspord);
            OnBeforeAuthenticate(loi);

            ISignInUserInfo result = null;

            IAuthenticator2 auth2 = this.GetAuthenticator2();

            if (auth2 != null)
            {
                SignInPerformanceCounters.DoAction(() => result = auth2.Authenticate(loi.LogOnName, loi.Password, loi.Context));
            }
            else
            {
                IAuthenticator auth = GetAuthenticator();
                SignInPerformanceCounters.DoAction(() => result = auth.Authenticate(loi.LogOnName, loi.Password));
            }

            return result;
        }

        private IAuthenticator GetAuthenticator()
        {
            return PassportSignInSettings.GetConfig().Authenticator;
        }

        private IAuthenticator2 GetAuthenticator2()
        {
            return PassportSignInSettings.GetConfig().Authenticator2;
        }
        #endregion
    }
}
