using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
    public partial class LinkButtonTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            bindingControl.Data = PrepareData();

        }

        private SimpleDataObject PrepareData()
        {
            SimpleDataObject data = new SimpleDataObject();
            data.SimpleDataType = DataType.Int;
            return data;

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bindingControl.CollectData();
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            bindingControl.CollectData();
            var value = OuUserInputControl1.SelectedSingleData;
        }

    }
}