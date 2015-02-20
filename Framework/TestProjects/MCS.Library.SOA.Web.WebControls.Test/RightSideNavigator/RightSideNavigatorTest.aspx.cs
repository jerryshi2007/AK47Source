using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.Web.WebControls.Test.RightSideNavigator
{
	public partial class RightSideNavigatorTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				RightSideNavigator1.RelativeLinks.Add(new WfRelativeLinkDescriptor("1") { Category = "StantardCate", Name = "百度", Url = "http://www.baidu.com" });
				RightSideNavigator1.RelativeLinks.Add(new WfRelativeLinkDescriptor("2") { Category = "MoreCate", Name = "更多>>", Url = "http://www.baidu.com" });
			}
		}
	}
}