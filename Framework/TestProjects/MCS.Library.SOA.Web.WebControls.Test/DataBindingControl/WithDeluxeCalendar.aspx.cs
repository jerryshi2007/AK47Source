using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
    public partial class WithDeluxeCalendar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            bindingControl.Data = PrepareData();
            //bindingControl.DataBind();

        }

        private SimpleDataObject PrepareData()
        {
            SimpleDataObject data = new SimpleDataObject();
            data.DateInput = DateTime.Now;
           
            return data;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            
            bindingControl.CollectData();
            var data = bindingControl.Data;
            calendarValueInServer.Text = DeluxeCalendar1.Value.ToString("yyyy-MM-dd");
        }
    }
}