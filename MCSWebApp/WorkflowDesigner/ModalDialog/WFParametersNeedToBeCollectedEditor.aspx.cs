using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Web;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WFParametersNeedToBeCollectedEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JSONSerializerExecute.RegisterConverter(typeof(WfParameterDescriptorConverter));
            WfParameterDescriptor wfpd=new WfParameterDescriptor();
            this.hiddenWfParameterTemplate.Value = JSONSerializerExecute.Serialize(wfpd);
        }
    }
}