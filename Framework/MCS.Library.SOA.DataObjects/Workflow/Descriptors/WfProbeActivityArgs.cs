using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 遍历活动时的回调参数
	/// </summary>
	public class WfProbeActivityArgs
	{
		/// <summary>
		/// 活动描述
		/// </summary>
		public IWfActivityDescriptor ActivityDescriptor
		{
			get;
			internal set;
		}

		/// <summary>
		/// 活动的级别
		/// </summary>
		public int Level
		{
			get;
			internal set;
		}
	}
}
