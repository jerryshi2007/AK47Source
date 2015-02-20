using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.OA.Web.WebControls.Test.UserControl
{
	public partial class testMergeResultOUGraphControl : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			StringBuilder strB = new StringBuilder();

			foreach (IOguObject obj in userMultiSelector.SelectedOuUserData)
			{
				if (strB.Length > 0)
					strB.Append("<br/>");

				strB.Append(HttpUtility.HtmlEncode(obj.FullPath));
			}

			result.InnerHtml = strB.ToString();

			base.OnPreRender(e);
		}
	}
}
