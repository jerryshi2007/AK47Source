using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class ClientGridUseSensitiveWordsTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var data = new List<MyData>();
                data.Add(new MyData() { CheckNo = true });
                gridTest.InitialData = data;
            }
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            var data = gridTest.InitialData;
            table1.Style["display"] = "none";
        }

    }



    public class MyData
    {
        public bool CheckNo { get; set; }

        public string Input
        {
            get;
            set;
        }
    }
}