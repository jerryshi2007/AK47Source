using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WcfExtensions;
using MCS.Library.Core;

namespace WfFormTemplate.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class WfProcessService : IWfProcessService
	{
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public DateTime GetServerTime()
		{
			return DateTime.Now;
		}

		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string[] StartBranchProcesses(string ownerActivityID, WfBranchProcessTransferParams branchTransferParams)
		{
			List<string> result = new List<string>();

			result.Add(UuidHelper.NewUuidString());
			result.Add(UuidHelper.NewUuidString());

			return result.ToArray();
		}
	}
}
