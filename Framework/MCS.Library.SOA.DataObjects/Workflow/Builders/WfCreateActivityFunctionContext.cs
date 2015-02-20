using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 创建活动时的函数上下文参数
	/// </summary>
	internal class WfCreateActivityFunctionContext
	{
		public WfCreateActivityFunctionContext(IWfProcessDescriptor processDesp, WfCreateActivityParamCollection createActivityParams, WfCreateActivityParam currentActivityParam)
		{
			this.ProcessDescriptor = processDesp;
			this.CreateActivityParams = createActivityParams;
			this.CurrentActivityParam = currentActivityParam;
		}

		public IWfProcessDescriptor ProcessDescriptor
		{
			get;
			private set;
		}

		public WfCreateActivityParamCollection CreateActivityParams
		{
			get;
			private set;
		}

		public WfCreateActivityParam CurrentActivityParam
		{
			get;
			private set;
		}
	}
}
