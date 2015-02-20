using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class clientGridStyleDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<myDemoEntity> list = new List<myDemoEntity>();

                for (int i = 0; i < 25; i++)
                {
                    myDemoEntity entity = new myDemoEntity();
                    entity.SelectedUserId = i.ToString();
                    entity.SelectedUserDisplayName = "SelectedUserDisplayName";
                    entity.SelectedUserUserFullPath = "SelectedUserUserFullPath";
                    list.Add(entity);
                }

                this.detailGrid.InitialData = list;
            }
        }
    }
}