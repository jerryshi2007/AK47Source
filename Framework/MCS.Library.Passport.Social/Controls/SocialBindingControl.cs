using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.Web.Controls
{
	[ToolboxData("<{0}:BindingControl runat=server></{0}:BindingControl>")]
	public class SocialBindingControl : Control
	{

		protected override void Render(HtmlTextWriter output)
		{
			if (this.DesignMode)
				output.Write("SocialBindingControl");
			else
				base.Render(output);
		}
	}
}
