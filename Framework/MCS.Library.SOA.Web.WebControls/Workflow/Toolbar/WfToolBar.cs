using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using MCS.Library.Core;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls
{
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:WfToolBar runat=server></{0}:WfToolBar>")]
	public class WfToolBar : WebControl
	{
		private System.Web.UI.Control templateControl = null;
		private string templatePath = string.Empty;

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

		protected override void CreateChildControls()
		{
			if (this.templatePath.IsNotEmpty())
			{
				this.templateControl = Page.LoadControl(this.templatePath);
			}
			else
			{ 
				string html = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), string.Format("{0}.Workflow.Toolbar.WfToolBarTemplate.htm", this.GetType().Namespace));
				this.templateControl = Page.ParseControl(html);
			}

			Controls.Add(this.templateControl);

			LinkButton toolbarRead = (LinkButton)MCS.Web.Library.WebControlUtility.FindControlByHtmlIDProperty(this, "toolbarRead", true);
			if (Page.Request["needReadBtn"] != null && Page.Request["taskID"] != null)
			{
				toolbarRead.Click += new EventHandler(WfReadControl.SetTaskAccomplishedAfterRead);
			}
			else
			{
				toolbarRead.Visible = false;
			}

			base.CreateChildControls();
		}

		protected override void OnInit(EventArgs e)
		{
			EnsureChildControls();
			base.OnInit(e);
		}
	}
}
