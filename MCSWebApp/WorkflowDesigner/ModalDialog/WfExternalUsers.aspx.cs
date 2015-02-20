using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfExternalUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             hiddenEnumGenderList.Value = JSONSerializerExecute.Serialize(EnumItemDescriptionAttribute.GetDescriptionList(typeof(MCS.Library.SOA.DataObjects.Gender)));

             //WfUserResourceDescriptor wfUserResDesc = new WfUserResourceDescriptor();
             //hiddenWfUserDescTemplate.Value = JSONSerializerExecute.Serialize(wfUserResDesc);
             WebUtility.RequiredScript(typeof(ClientGrid));

        }
    }
}