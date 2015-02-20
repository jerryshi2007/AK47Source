using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace MCS.Web.Responsive.WebControls.Test.DeluxeMenu
{
    public partial class DeluxeMenuTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddSome(object sender, EventArgs e)
        {
            this.menu1.Items.Add(new DeluxeMenuStripItem() { Text = "Added At" + this.menu1.Items.Count });

            System.IO.StringWriter w = new System.IO.StringWriter();
            Console.SetOut(w);
            Console.WriteLine("Hello");
            Console.WriteLine("World");
            var result = w.ToString();

            Page.MaintainScrollPositionOnPostBack = true;
        }
    }
}