using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// ComponentHelper的包装类
	/// </summary>
    [ToolboxData("<{0}:ComponentHelperWrapper runat=server></{0}:ComponentHelperWrapper>")]
	public class ComponentHelperWrapper : Control
	{
		private const string ControlID = "componentHelperActiveX";

		public ComponentHelperWrapper()
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			ComponentHelperWrapperSettings config = ComponentHelperWrapperSettings.GetConfig();
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), ControlID,
											string.Format(
												"<object id=\"{0}\" codebase=\"{1}\" height=\"0\" width=\"0\" classid=\"{2}\" style=\"display: none\"></object>",
												HttpUtility.HtmlAttributeEncode(ControlID),
												HttpUtility.HtmlAttributeEncode(config.Codebase),
												HttpUtility.HtmlAttributeEncode(config.ClassID)
											 ),
											false);

			base.OnPreRender(e);
		}
	}
}

