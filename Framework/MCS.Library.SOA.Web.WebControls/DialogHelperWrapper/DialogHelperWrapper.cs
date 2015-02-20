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
    /// DialogHelper的包装类
    /// </summary>
    [ToolboxData("<{0}:DialogHelperWrapper runat=server></{0}:DialogHelperWrapper>")]
    public class DialogHelperWrapper : Control
    {
        private const string ControlID = "dialogHelperActiveX";

        public DialogHelperWrapper()
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            DialogHelperWrapperSettings config = DialogHelperWrapperSettings.GetConfig();
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
