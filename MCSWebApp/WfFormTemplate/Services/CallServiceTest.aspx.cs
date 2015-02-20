using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;

namespace WfFormTemplate.Services
{
	public partial class CallServiceTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void buttonGetServiceName_Click(object sender, EventArgs e)
		{
			WfServiceAddressDefinition address = new WfServiceAddressDefinition(WfServiceRequestMethod.Post,
				"http://localhost/MCSWebApp/WfFormTemplate/Services/WfProcessService.svc",
				WfServiceContentType.Json);

			WfServiceOperationDefinition definition = new WfServiceOperationDefinition(address, "GetServerTime");
			WfServiceInvoker invoker = new WfServiceInvoker(definition);

			object result = invoker.Invoke();

			resultLabel.Text = Server.HtmlEncode(result.ToString());
		}

		protected void buttonCallBranchProcess_Click(object sender, EventArgs e)
		{
			WfServiceAddressDefinition address = new WfServiceAddressDefinition(WfServiceRequestMethod.Post,
				"http://localhost/MCSWebApp/WfFormTemplate/Services/WfProcessService.svc",
				WfServiceContentType.Json);

			WfServiceOperationDefinition definition = new WfServiceOperationDefinition(address, "StartBranchProcesses");

			WfServiceOperationParameter p1 = new WfServiceOperationParameter("ownerActivityID", UuidHelper.NewUuidString());

			IWfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor("Test");
			WfBranchProcessTransferParams transferParams = new WfBranchProcessTransferParams(template);

			WfServiceOperationParameter p2 = new WfServiceOperationParameter("branchTransferParams", WfSvcOperationParameterType.RuntimeParameter, "transferParams");

			definition.Params.Add(p1);
			definition.Params.Add(p2);

			WfServiceInvoker invoker = new WfServiceInvoker(definition);

			WfServiceInvoker.InvokeContext["transferParams"] = transferParams;

			object result = invoker.Invoke();

			resultLabel.Text = Server.HtmlEncode(result.ToString());
		}
	}
}