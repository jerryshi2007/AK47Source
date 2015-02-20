using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	[Serializable]
	public abstract class InvokeProcessServiceTaskBase : InvokeServiceTask
	{
		protected InvokeProcessServiceTaskBase()
			: base()
		{
			WfConverterHelper.RegisterConverters();

			this.Category = "InvokeProcessService";
		}

		protected InvokeProcessServiceTaskBase(SysTask other)
			: base(other)
		{
		}

		protected virtual void InitServiceDefinitions()
		{
			WfServiceAddressDefinition address = new WfServiceAddressDefinition(WfServiceRequestMethod.Post, string.Empty, WfServiceContentType.Json);
			this.PrepareAddress(address);

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			WfServiceOperationDefinition definition = new WfServiceOperationDefinition(address, this.GetOperationName(), parameters, "returnValue");

			this.PrepareParameters(parameters);

			this.SvcOperationDefs.Add(definition);
		}

		protected virtual void PrepareAddress(WfServiceAddressDefinition address)
		{
			address.Address = ResourceUriSettings.GetConfig().Paths.CheckAndGet("wfProcessService").Uri.ToUriString();
		}

		protected abstract string GetOperationName();

		protected virtual void PrepareParameters(WfServiceOperationParameterCollection parameters)
		{
		}
	}
}
