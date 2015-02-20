using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.HBGrid
{
    public partial class TestWithTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var list = new List<DataEntity>();
                list.Add(new DataEntity());
                gridWorkPlan.InitialData = list;
            }
        }

        class DataEntity
        {
            public bool HasCashPoolChoice { get; set; }
        }
    }
}