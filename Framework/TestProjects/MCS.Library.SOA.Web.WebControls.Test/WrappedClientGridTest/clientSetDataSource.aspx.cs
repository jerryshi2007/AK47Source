using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class clientSetDataSource : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //myDemoUser entity = new myDemoUser();
            //entity.SelectedUserDisplayName = "懂";
            //entity.SelectedUserId = "feeeeeee11";
            //entity.SelectedUserUserFullPath = "feeeeeeaaaaaaa";

            //string f = JSONSerializerExecute.Serialize(entity, entity.GetType());

            if (!IsPostBack)
            {
                List<myDemoUser> list = new List<myDemoUser>();
                this.detailGrid.InitialData = list;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var data = this.detailGrid.InitialData;
        }
    }

    public class myDemoUser
    {
        public string SelectedUserId { get; set; }
        public string SelectedUserDisplayName { get; set; }
        public string SelectedUserUserFullPath { get; set; }
    }
}