using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
	public partial class dropdownListBindingTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			bindingControl.Data = SimpleUserAdapter.PrepareUsers();
		}

		protected override void OnPreRenderComplete(EventArgs e)
		{
			this.DataBind();

			selectedCadidateID.Text = HttpUtility.HtmlEncode(candidates.SelectedValue);

			base.OnPreRenderComplete(e);
		}
	}
}