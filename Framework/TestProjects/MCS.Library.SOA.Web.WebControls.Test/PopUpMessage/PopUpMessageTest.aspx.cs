using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Library.SOA.Web.WebControls.Test.PopUpMessage
{
	public partial class PopUpMessageTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			 
		 
			this.PopUpMessageControl1.ShowTime = new TimeSpan(0,0,4);
		 
		 

			//this.PopUpMessageControl1.PositionElementID = "div1";
			//this.PopUpMessageControl1.PositionX = 0;
			//this.PopUpMessageControl1.PositionY = 0;
		}
	}
}