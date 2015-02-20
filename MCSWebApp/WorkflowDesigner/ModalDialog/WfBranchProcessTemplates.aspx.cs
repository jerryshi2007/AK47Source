using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;

using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Web.Library;
using MCS.Web.WebControls;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfBranchProcessTemplates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(ClientGrid));
            var blockingTypeList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfBranchProcessBlockingType));

            var executeSequenceList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfBranchProcessExecuteSequence));
            hiddenWfBranchProcessBlockingType.Value = JSONSerializerExecute.Serialize(blockingTypeList);
            hiddenWfBranchProcessExecuteSequence.Value = JSONSerializerExecute.Serialize(executeSequenceList);
        }
    }
}