﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
	[Serializable]
	public class LeaveActivityInvokeServiceAction : ActivityInvokeServiceActionBase
	{
		protected override WfServiceOperationDefinitionCollection GetOperationsBeforePersist()
		{
			return WfRuntime.ProcessContext.CurrentActivity.Descriptor.LeaveEventExecuteServices.GetServiceOperationsBeforePersist();
		}

		protected override WfServiceOperationDefinitionCollection GetOperationsWhenPersist()
		{
			return WfRuntime.ProcessContext.CurrentActivity.Descriptor.LeaveEventExecuteServices.GetServiceOperationsWhenPersist();
		}
	}
}
