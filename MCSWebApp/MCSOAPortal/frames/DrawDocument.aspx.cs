using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Passport;

namespace MCS.OA.Portal.frames
{
    public partial class DrawDocument : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("HRUser"))
            //{
            //    this.hrEmployeeInformation.Visible = true;
            //    this.hrEmployeeEntry.Visible = true;
            //}
            //else
            //{
            //    this.hrEmployeeInformation.Visible = false;
            //    this.hrEmployeeEntry.Visible = false;
            //}
        }
    }
}