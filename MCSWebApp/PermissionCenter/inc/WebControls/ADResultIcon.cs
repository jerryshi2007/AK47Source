using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PermissionCenter.WebControls
{
	[DefaultProperty("Status")]
	[ToolboxData("<{0}:ResultIcon runat=server></{0}:ResultIcon>")]
	public class ADResultIcon : System.Web.UI.HtmlControls.HtmlControl
	{
		public ADResultIcon()
			: base("span")
		{
		}

		/// <summary>
		/// 0正常，1异常退出，2失败
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("状态码")]
		public int Status
		{
			get
			{
				object o = this.ViewState["Status"];
				return (o == null) ? 0 : (int)o;
			}

			set
			{
				this.ViewState["Status"] = value;
			}
		}

		/// <summary>
		/// 图标模式
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("呈现为文本")]
		public bool TextOnly
		{
			get
			{
				object o = this.ViewState["TextOnly"];
				return (o == null) ? false : (bool)o;
			}

			set
			{
				this.ViewState["TextOnly"] = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.TextOnly == false)
			{
				this.SetAttribute("data-status", this.Status.ToString());
				this.SetAttribute("class", this.GetClassName());
			}

			this.SetAttribute("title", this.GetTitle());
		}

		public override void RenderControl(HtmlTextWriter writer)
		{
			base.RenderControl(writer);
		}

		protected override void RenderBeginTag(HtmlTextWriter writer)
		{
			base.RenderBeginTag(writer);

			if (this.TextOnly)
			{
				writer.WriteEncodedText(this.GetTitle());
			}
		}

		private string GetTitle()
		{
			switch (this.Status)
			{
				case 0:
					return "成功";
				case 1:
					return "执行中出错";
				case 2:
					return "执行失败";
				case 3:
					return "执行中";
				default:
					return "未知状态";
			}
		}

		private string GetClassName()
		{
			switch (this.Status)
			{
				case 0:
					return "pc-adresult pc-status-ok";
				case 1:
					return "pc-adresult pc-status-warning";
				case 2:
					return "pc-adresult pc-status-error";
				case 3:
					return "pc-adresult pc-status-running";
				default:
					return string.Empty;
			}
		}
	}
}
