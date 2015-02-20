using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.CustomerServiceExecutiveLink.CustomerServiceExecutiveLink.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.CustomerServiceExecutiveLink.CustomerServiceExecutiveLink.css", "text/css", PerformSubstitution = true)]

namespace MCS.Web.WebControls
{
	[ClientCssResource("MCS.Web.WebControls.CustomerServiceExecutiveLink.CustomerServiceExecutiveLink.css")]
	[ClientScriptResource("MCS.Web.WebControls.CustomerServiceExecutiveLink",
		"MCS.Web.WebControls.CustomerServiceExecutiveLink.CustomerServiceExecutiveLink.js")]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	public class CustomerServiceExecutiveLink : ScriptControlBase
	{
		private static readonly string ControlKey = "CustomerServiceExecutiveLink";

		[Category("Default")]
		[Description("分类")]
		public string Category
		{
			get { return GetPropertyValue("Category", string.Empty); }
			set { SetPropertyValue("Category", value); }
		}

		public CustomerServiceExecutiveLink()
			: base(false, HtmlTextWriterTag.Div)
		{
			base.CssClass = "csel_services";
		}

		protected override void OnInit(EventArgs e)
		{
			ExceptionHelper.TrueThrow(HttpContext.Current.Items.Contains(ControlKey), "页面中只能有一个CustomerServiceExecutiveLink控件！");
			HttpContext.Current.Items.Add(ControlKey, true);
			base.OnInit(e);
		}


		protected override void OnPreRender(EventArgs e)
		{
			if (!DesignMode)
			{
				var users =
					CutomerServiceExecutiveSetting.GetConfig().CutomerServiceExecutiveQuery.QueryCutomerServiceExecutive(Category.Split(','));

				if (users.Any())
				{
					var title = new HtmlGenericControl("Span");
					title.Attributes.Add("class", "csel_more");
					title.InnerText = CutomerServiceExecutiveSetting.GetConfig().Title;
					this.Controls.Add(title);



					foreach (var u in users)
					{
						var userName = new HtmlGenericControl("Span");
						userName.Attributes.Add("class", "csel_list");

						UserPresence presence = new UserPresence();

						presence.UserID = u.ID;
						presence.UserDisplayName = u.DisplayName;

						userName.Controls.Add(presence);
						this.Controls.Add(userName);

						presence.EnsureInUserList();
					}
				}
			}
			base.OnPreRender(e);
		}
	}
}
