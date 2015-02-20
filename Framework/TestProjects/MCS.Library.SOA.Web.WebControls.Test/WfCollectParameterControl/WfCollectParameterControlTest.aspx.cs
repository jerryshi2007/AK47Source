using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class WfCollectParameterControlTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack == false)
            {
                //this.testControl.Items.Add(new ListItem("测试一", "1"));
                //this.testControl.Items.Add(new ListItem("测试二", "2"));
                //this.testControl.Items.Add(new ListItem("测试三", "3"));
            }
        }

        protected void bt_s_Click(object sender, EventArgs e)
        {
            // this.collectParameterControl.CollectData();
        }

        //protected override void OnPreRenderComplete(EventArgs e)
        //{
        //    this.DataBind();
        //    base.OnPreRenderComplete(e);
        //}
    }
}