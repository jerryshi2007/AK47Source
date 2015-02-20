using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using System.Text;

namespace MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl
{
	public partial class testVisibility : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void setVisibleBtn_Click(object sender, EventArgs e)
		{
			userSelector.Visible = !userSelector.Visible;
		}

		protected override void OnPreRender(EventArgs e)
		{
			StringBuilder strB = new StringBuilder();

			foreach (IOguObject obj in userSelector.SelectedOuUserData)
			{
				if (strB.Length > 0)
					strB.Append("<br/>");

				strB.Append(HttpUtility.HtmlEncode(obj.FullPath));
			}

			this.result.Text = strB.ToString();
			base.OnPreRender(e);
		}
	}
}