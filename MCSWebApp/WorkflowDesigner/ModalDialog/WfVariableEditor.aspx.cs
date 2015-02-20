using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfVariableEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //给流程中变量描述赋值
            string guid = Guid.NewGuid().ToString();
            WfVariableDescriptor variDesc = new WfVariableDescriptor();

            hiddenVariableTemplate.Value = JSONSerializerExecute.Serialize(variDesc);
            BindList(ddlVariableDatatype,EnumItemDescriptionAttribute.GetDescriptionList(typeof(MCS.Library.SOA.DataObjects.Workflow.DataType)));
        }

        private void BindList(DropDownList ddl, EnumItemDescriptionList list)
        {
            ddl.DataSource = list;
            ddl.DataTextField = "Description";
			ddl.DataValueField = "EnumValue";
            ddl.DataBind();
        }

    }
}