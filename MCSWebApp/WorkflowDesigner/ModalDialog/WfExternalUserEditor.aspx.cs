using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfExternalUserEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WfExternalUser extUser = new WfExternalUser();
            hiddenExtUserTemplate.Value = JSONSerializerExecute.Serialize(extUser);
            BindList(ddlGender, EnumItemDescriptionAttribute.GetDescriptionList(typeof(MCS.Library.SOA.DataObjects.Gender)));
        }
        private void BindList(DropDownList ddl, EnumItemDescriptionList list)
        {
            ddl.DataSource = list;
            ddl.DataTextField = "Description";
            ddl.DataValueField = "EnumValue";
            ddl.DataBind();
            ddl.Items.Insert(0, (new ListItem("", "")));
        }

    }
}