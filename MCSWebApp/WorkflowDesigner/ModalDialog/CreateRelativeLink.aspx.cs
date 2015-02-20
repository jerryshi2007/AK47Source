using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.ModalDialog
{
	public partial class CreateRelativeLink : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WfRelativeLinkDescriptor rlink = new WfRelativeLinkDescriptor("");
			WfConverterHelper.RegisterConverters();
			this.relativeLinkTemplate.Value = JSONSerializerExecute.Serialize(rlink);
		}
	}
}