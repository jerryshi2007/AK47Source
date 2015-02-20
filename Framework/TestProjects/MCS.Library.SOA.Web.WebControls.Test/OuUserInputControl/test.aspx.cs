using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;


namespace MCS.Library.SOA.Web.WebControls.Test.OuUserInputControl
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			//if (!IsPostBack)
			//{
                OguUser user = new OguUser("80f4464f-e912-40c9-9502-c369a0d935ee");

                OguDataCollection<IOguObject> list = new OguDataCollection<IOguObject>();
                list.Add(user);

                this.OuUserInputControl1.SelectedOuUserData = list;
			//}
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //this.OuUserInputControl1.Enabled = false;
        }

		protected void Button2_Click(object sender, EventArgs e)
		{
			
		}
    }
}