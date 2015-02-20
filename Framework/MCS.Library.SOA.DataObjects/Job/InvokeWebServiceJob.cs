using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.JOB_INVOKE_SERVICE")]
	public class InvokeWebServiceJob : JobBase
	{
		public InvokeWebServiceJob()
			: base()
		{
			this.JobType = JobType.InvokeService;
		}

		[ORFieldMapping("SERVICE_DEF_DATA")]
		public string XmlData
		{
			get;
			set;
		}

		private WfServiceOperationDefinitionCollection _SvcOperationDefs;
		public WfServiceOperationDefinitionCollection SvcOperationDefs
		{
			get
			{
				if (this._SvcOperationDefs == null)
				{
					this._SvcOperationDefs = new WfServiceOperationDefinitionCollection();
				}
				return _SvcOperationDefs;
			}
			set
			{
				_SvcOperationDefs = value;
			}
		}

		public override void Start()
		{
			foreach (WfServiceOperationDefinition svcDefinition in SvcOperationDefs)
			{
				WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);

				svcInvoker.Headers.Set("InvokeWebServiceJobID", this.JobID);
				object result = svcInvoker.Invoke();

				if (svcDefinition.RtnXmlStoreParamName.IsNotEmpty())
					WfServiceInvoker.InvokeContext[svcDefinition.RtnXmlStoreParamName] = result;
			}
		}
	}

	[Serializable]
	public class InvokeWebServiceJobCollection : EditableKeyedDataObjectCollectionBase<string, InvokeWebServiceJob>
	{
		protected override string GetKeyForItem(InvokeWebServiceJob item)
		{
			return item.JobID;
		}
	}
}
