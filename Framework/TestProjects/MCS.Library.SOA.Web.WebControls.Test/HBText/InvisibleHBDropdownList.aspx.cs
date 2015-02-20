using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBText
{
	public partial class InvisibleHBDropdownList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			selectedText.Text = HttpUtility.HtmlEncode(dropdownList.SelectedText);
		}
	}
}