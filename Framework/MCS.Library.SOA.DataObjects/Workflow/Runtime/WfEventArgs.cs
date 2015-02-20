using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 为WfProcessActionContext的事件准备的参数
	/// </summary>
	public class WfMoveToEventArgs : System.EventArgs
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public WfMoveToEventArgs()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="fromActivity"></param>
		/// <param name="toActivity"></param>
		/// <param name="transferParams"></param>
		public WfMoveToEventArgs(IWfActivity fromActivity, IWfActivity toActivity, WfTransferParams transferParams)
		{
			this.FromActivity = fromActivity;
			this.ToActivity = toActivity;
			this.TransferParams = transferParams;
		}

		/// <summary>
		/// 从哪一个活动开始流转。在流程启动时，这个属性为null
		/// </summary>
		public IWfActivity FromActivity
		{
			get;
			internal set;
		}

		/// <summary>
		/// 流转到哪一个活动
		/// </summary>
		public IWfActivity ToActivity
		{
			get;
			internal set;
		}

		/// <summary>
		/// 流程流转的参数，包括从哪一条线流转过来、分支流程参数等信息，都包含在里面
		/// </summary>
		public WfTransferParams TransferParams
		{
			get;
			internal set;
		}
	}
}
