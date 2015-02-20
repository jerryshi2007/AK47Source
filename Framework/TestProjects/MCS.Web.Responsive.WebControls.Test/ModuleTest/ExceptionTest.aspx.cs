using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls.Test.ModuleTest
{
	public partial class ExceptionTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void throwException_Click(object sender, EventArgs e)
		{
			throw new ApplicationException("异常测试");
		}
	}
}