using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test.DownloadFile
{
	public partial class downloadFileTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtility.RequiredScript(typeof(HBCommonScript));

			if (Request.HttpMethod == "POST")
			{
				Response.AppendHeader("content-disposition", "attachment;fileName=" + UuidHelper.NewUuidString() + "*.txt");

				Response.Write(string.Format("Hello {0}. {1}", UserName.Text, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
				Response.End();
			}
		}
	}
}