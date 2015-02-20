using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.Validation
{
	public partial class TextValidatorTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(HBCommonScript));
		}
	}
}