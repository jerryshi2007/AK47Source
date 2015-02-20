using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCSResponsiveOAPortal
{
    public partial class Default : Page
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("~/Task/TodoList.aspx", true);
            
        }
    }
}