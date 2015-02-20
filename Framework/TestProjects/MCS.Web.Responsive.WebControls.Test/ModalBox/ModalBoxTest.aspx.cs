using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Resources;

namespace MCS.Web.Responsive.WebControls.Test.ModalBox
{
    public partial class ModalBoxTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(ClientMsgResources));
        }
    }
}