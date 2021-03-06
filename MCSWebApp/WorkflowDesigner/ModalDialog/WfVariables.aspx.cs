﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ModalDialog
{
    public partial class WfVariables : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(ClientGrid));
            hiddenDatatypeJson.Value = JSONSerializerExecute.Serialize(EnumItemDescriptionAttribute.GetDescriptionList(typeof(MCS.Library.SOA.DataObjects.Workflow.DataType)));
        }
    }
}