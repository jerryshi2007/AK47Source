using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;
using MCS.Web.Responsive.Library;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 访问票据的检查器
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:AccessTicketChecker runat=server></{0}:AccessTicketChecker>")]
	public class AccessTicketChecker : Control
	{
		/// <summary>
		/// 票据检查出错时的事件
		/// </summary>
		public event AccessTicketCheckHandler TicketChecking;

		/// <summary>
		/// 检查票据的阶段。默认是Init阶段
		/// </summary>
		[DefaultValue(AccessTicketCheckPhase.Init)]
		public AccessTicketCheckPhase CheckPhase
		{
			get
			{
				AccessTicketCheckPhase result = AccessTicketCheckPhase.Init;

				if (ViewState["CheckPhase"] != null)
					result = (AccessTicketCheckPhase)ViewState["CheckPhase"];

				return result;
			}
			set
			{
				ViewState["CheckPhase"] = value;
			}
		}

		/// <summary>
		/// 是否检查url的合法性
		/// </summary>
		[DefaultValue(true)]
		public bool CheckUrl
		{
			get
			{
				bool result = AccessTicketSettings.GetConfig().CheckUrl;

				if (ViewState["CheckUrl"] != null)
					result = (bool)ViewState["CheckUrl"];

				return result;
			}
			set
			{
				ViewState["CheckUrl"] = value;
			}
		}

		/// <summary>
		/// 需要检查的Url的部分
		/// </summary>
		[DefaultValue(AccessTicketUrlCheckParts.All)]
		public AccessTicketUrlCheckParts UrlCheckParts
		{
			get
			{
				AccessTicketUrlCheckParts result = AccessTicketSettings.GetConfig().UrlCheckParts;

				if (ViewState["UrlCheckParts"] != null)
					result = (AccessTicketUrlCheckParts)ViewState["UrlCheckParts"];

				return result;
			}
			set
			{
				ViewState["UrlCheckParts"] = value;
			}
		}

		/// <summary>
		/// 是否启用
		/// </summary>
		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				bool result = AccessTicketSettings.GetConfig().Enabled;

				if (ViewState["Enabled"] != null)
					result = (bool)ViewState["Enabled"];

				return result;
			}
			set
			{
				ViewState["Enabled"] = value;
			}
		}

		/// <summary>
		/// 过期时间
		/// </summary>
		public TimeSpan Timeout
		{
			get
			{
				TimeSpan result = AccessTicketSettings.GetConfig().TicketTimeout;

				if (ViewState["Timeout"] != null)
					result = (TimeSpan)ViewState["Timeout"];

				return result;
			}
			set
			{
				ViewState["Timeout"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			CheckAccessTicket(AccessTicketCheckPhase.Init);

			base.OnInit(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			CheckAccessTicket(AccessTicketCheckPhase.Load);

			base.OnLoad(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			CheckAccessTicket(AccessTicketCheckPhase.PreRender);

			base.OnPreRender(e);
		}

		/// <summary>
		/// 渲染
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output)
		{
			if (this.DesignMode == true)
				output.Write("访问票据检查器");
		}

		/// <summary>
		/// 票据检查
		/// </summary>
		protected virtual void OnTicketChecking(AccessTicketCheckEventArgs eventArgs)
		{
			if (TicketChecking != null)
				TicketChecking(this, eventArgs);
		}

		private void CheckAccessTicket(AccessTicketCheckPhase phase)
		{
			if (this.Enabled && IsCheckPhaseMatched(phase))
			{
				PageRenderMode renderMode = Request.GetRequestPageRenderMode();

				if (renderMode == null || renderMode.UseNewPage == false)
				{
					Uri matchedUrl = null;

					if (this.CheckUrl)
						matchedUrl = this.Page.Request.Url;

					AccessTicketCheckEventArgs eventArgs = null;

					try
					{
						AccessTicket ticket = AccessTicketManager.CheckAccessTicket(matchedUrl, this.UrlCheckParts, this.Timeout);

						eventArgs = new AccessTicketCheckEventArgs(ticket, true, string.Empty);
					}
					catch (AccessTicketCheckException ex)
					{
						AccessTicket ticket = AccessTicketManager.GetAccessTicket();

						eventArgs = new AccessTicketCheckEventArgs(ticket, false, ex.Message);
					}

					OnTicketChecking(eventArgs);

					if (eventArgs.IsValid == false)
						throw new AccessTicketCheckException(eventArgs.ErrorMessage);
				}
			}
		}

		/// <summary>
		/// 是否是需要检查票据的阶段
		/// </summary>
		/// <returns></returns>
		private bool IsCheckPhaseMatched(AccessTicketCheckPhase phase)
		{
			bool result = (CheckPhase & phase) != AccessTicketCheckPhase.None;

			if (result && string.Compare(Page.Request.HttpMethod, "POST", true) == 0)
				result = (CheckPhase & AccessTicketCheckPhase.Post) != AccessTicketCheckPhase.None;

			if (result && Page.IsCallback)
				result = (CheckPhase & AccessTicketCheckPhase.CallBack) != AccessTicketCheckPhase.None;

			return result;
		}
	}
}
