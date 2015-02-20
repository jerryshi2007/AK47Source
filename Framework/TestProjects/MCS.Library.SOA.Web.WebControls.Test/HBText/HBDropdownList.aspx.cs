using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBText
{
    public partial class HBDropdownList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			if (this.IsPostBack == false)
			{
				dropdownList.Items.Add(new ListItem("One", "1"));
				dropdownList.Items.Add(new ListItem("Two", "2"));
			}
        }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			selectedText.Text = HttpUtility.HtmlEncode(dropdownList.SelectedText);
		}
    }
}