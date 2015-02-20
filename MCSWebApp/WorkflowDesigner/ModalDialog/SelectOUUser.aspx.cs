using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ModalDialog
{
    public partial class SelectOUUser : System.Web.UI.Page
    {
        protected string sResType;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            int resourceType = Int32.Parse(Request.Params["resourceType"]);
             UserControlObjectMask resType = (UserControlObjectMask)resourceType;
             OuUserInputControl.SelectMask = resType;
            //if (isOrg)
            //{
            //    sResType = "部门  ";
            //    OuUserInputControl.SelectMask = MCS.OA.Web.WebControls.UserControlObjectMask.Organization;
            //}
            //else
            //{
            //    sResType = "用户";
            //    OuUserInputControl.SelectMask = MCS.OA.Web.WebControls.UserControlObjectMask.All;
            //}
        }
    }
}