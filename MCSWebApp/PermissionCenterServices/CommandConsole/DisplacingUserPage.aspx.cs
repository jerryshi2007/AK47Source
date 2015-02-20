using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Principal;

namespace PermissionCenter.CommandConsole
{
	public partial class DisplacingUserPage : System.Web.UI.Page
	{
		public override void ProcessRequest(HttpContext context)
		{
			if (false == MCS.Library.SOA.DataObjects.Security.Permissions.SCPrincipalExtension.IsSupervisor(DeluxePrincipal.Current))
			{
				throw new HttpException("只有管理员允许查看此页面");
			}
			else
			{
				base.ProcessRequest(context);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}