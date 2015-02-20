using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 仿真步骤的执行结果
	/// </summary>
	[Serializable]
	public class WfSimulationResult
	{
		public WfSimulationResult()
		{
		}

		public WfSimulationResult(IWfProcess process)
		{
			process.NullCheck("process");

			this.ProcessID = process.ID;

			if (process.CurrentActivity != null)
				this.ActivityID = process.CurrentActivity.ID;

			this.MoveToCount = WfRuntime.ProcessContext.SimulationContext.MoveToCount;
			this.ProcessStatus = process.Status;
		}

		/// <summary>
		/// 流程ID
		/// </summary>
		public string ProcessID
		{
			get;
			internal set;
		}

		/// <summary>
		/// 活动ID
		/// </summary>
		public string ActivityID
		{
			get;
			internal set;
		}

		/// <summary>
		/// 流转次数
		/// </summary>
		public int MoveToCount
		{
			get;
			internal set;
		}

		/// <summary>
		/// 流程状态
		/// </summary>
		public WfProcessStatus ProcessStatus
		{
			get;
			internal set;
		}

		/// <summary>
		/// 输出字符串
		/// </summary>
		public string OutputString
		{
			get;
			internal set;
		}
	}
}
