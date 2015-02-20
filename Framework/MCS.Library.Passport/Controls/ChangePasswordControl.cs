using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using MCS.Library.Passport;
using MCS.Library.Passport.Properties;
using MCS.Library.Core;

namespace MCS.Library.Web.Controls
{
	/// <summary>
	/// 修改用户密码的控件
	/// </summary>
	[ToolboxData("<{0}:ChangePasswordControl runat=server></{0}:ChangePasswordControl>")]
	public class ChangePasswordControl : WebControl
	{
		private System.Web.UI.Control templateControl = null;
		private string templatePath = string.Empty;
		private Button updateButton = null;
		private string referUrl = string.Empty;

		#region Properties
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
		/// 成功以后返回的Url
		/// </summary>
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string SuccessUrl
		{
			get
			{
				return PassportWebControlHelper.GetViewState(ViewState, "SuccessUrl", string.Empty);
			}
			set
			{
				ViewState["SuccessUrl"] = value;
			}
		}

		/// <summary>
		/// 返回的Url
		/// </summary>
		public string BackUrl
		{
			get
			{
				return PassportWebControlHelper.GetViewState(ViewState, "BackUrl", string.Empty);
			}
			set
			{
				ViewState["BackUrl"] = value;
			}
		}

		/// <summary>
		/// 自动重定向到Refer页面
		/// </summary>
		[DefaultValue(true)]
		public bool AutoRedirect
		{
			get
			{
				return PassportWebControlHelper.GetViewState(ViewState, "AutoRedirect", true);
			}
			set
			{
				ViewState["AutoRedirect"] = value;
			}
		}

		/// <summary>
		/// 参照的Url
		/// </summary>
		private string ReferUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.referUrl))
				{
					Uri refer = HttpContext.Current.Request.UrlReferrer;

					this.referUrl = PassportWebControlHelper.GetViewState(ViewState,
						"ReferUrl",
						refer != null ? refer.ToString() : string.Empty);
				}

				return this.referUrl;
			}
			set
			{
				ViewState["ReferUrl"] = value;
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
				EnsureChildControls();
				return this.templateControl;
			}
		}

		#endregion

		/// <summary>
		/// 重载OnLoad
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			EnsureChildControls();
			base.OnLoad(e);
		}

		/// <summary>
		/// 创建子控件
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			if (TemplatePath != string.Empty)
			{
				this.templateControl = Page.LoadControl(TemplatePath);

				this.Controls.Add(this.templateControl);

				Initialize();
			}
		}

		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			ViewState["ReferUrl"] = this.ReferUrl;

			InitScript();
			base.OnPreRender(e);
		}

		#region Private
		private string GetBackUrl()
		{
			string result = string.Empty;

			if (string.IsNullOrEmpty(this.BackUrl))
				result = this.ReferUrl;
			else
				result = this.BackUrl;

			return result;
		}

		private void Initialize()
		{
			this.updateButton = (Button)PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "changePasswordButton");

			if (this.updateButton != null)
				this.updateButton.Click += new EventHandler(updateButton_Click);

			if (this.Page.IsPostBack == false)
			{
				if (HttpContext.Current.User != null)
					PassportWebControlHelper.SetControlValue(this.TemplateControl, "signInName", "Text", HttpContext.Current.User.Identity.Name);
			}
		}

		private void InitScript()
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SignInScriptOnload", Resource.changePasswordScript, true);

			Page.ClientScript.RegisterStartupScript(this.GetType(), "ChangePasswordScriptOnload",
				string.Format("window.attachEvent(\"onload\", new Function(\"initControlsFocus('{0}', '{1}')\"));",
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "ClientID", string.Empty),
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "oldPassword", "ClientID", string.Empty)), true);

			Control detailErrorMessageLink = PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "detailErrorMessageLink");

			if (detailErrorMessageLink != null && detailErrorMessageLink is HtmlContainerControl)
				((HtmlContainerControl)detailErrorMessageLink).Attributes["onclick"] =
					string.Format("doDetailErrorMessageClick(\"{0}\")",
					PassportWebControlHelper.GetControlValue(this.TemplateControl, "detailErrorMessage", "ClientID", string.Empty));

			this.updateButton.OnClientClick = string.Format("beforeSubmit(\"{0}\", \"{1}\", \"{2}\", \"{3}\")",
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "ClientID", string.Empty),
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "errorMessage", "ClientID", string.Empty),
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "newPassword", "ClientID", string.Empty),
				PassportWebControlHelper.GetControlValue(this.TemplateControl, "confirmPassword", "ClientID", string.Empty));

			IAttributeAccessor backButton = (IAttributeAccessor)PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "backButton");

			if (backButton != null)
			{
				backButton.SetAttribute("backUrl", this.GetBackUrl());
				backButton.SetAttribute("onclick", "onBackButtonClick();");
			}
		}

		private void updateButton_Click(object sender, EventArgs e)
		{
			try
			{
				string userID = PassportWebControlHelper.GetControlValue(this.TemplateControl, "signInName", "Text", string.Empty);
				string oldPassword = PassportWebControlHelper.GetControlValue(this.TemplateControl, "oldPassword", "Value", string.Empty);
				string newPassword = PassportWebControlHelper.GetControlValue(this.TemplateControl, "newPassword", "Value", string.Empty);

				ChangeUserPassword(userID, oldPassword, newPassword);

				if (AutoRedirect)
				{
					HttpContext.Current.Response.Redirect(this.ReferUrl, true);
				}
				else
				{
					if (string.IsNullOrEmpty(this.SuccessUrl) == false)
						HttpContext.Current.Response.Redirect(this.SuccessUrl, true);
				}
			}
			catch (System.Exception ex)
			{
				PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "Text",
					HttpUtility.HtmlEncode(ExceptionHelper.GetRealException(ex).Message));
				PassportWebControlHelper.SetControlValue(this.TemplateControl, "errorMessage", "ForeColor", Color.Red);
				PassportWebControlHelper.SetControlValue(this.TemplateControl, "detailErrorMessage", "Text", HttpUtility.HtmlEncode(ex.StackTrace));

				Control detailErrorMessageLink = PassportWebControlHelper.FindControlRecursively(this.TemplateControl, "detailErrorMessageLink");

				if (detailErrorMessageLink != null && detailErrorMessageLink is HtmlContainerControl)
					((HtmlContainerControl)detailErrorMessageLink).Style["display"] = "block";
			}
		}

		private static void ChangeUserPassword(string userID, string oldPassword, string newPassword)
		{
			PassportSignInSettings.GetConfig().UserInfoUpdater.ChangePassword(userID, oldPassword, newPassword);
		}
		#endregion
	}
}
