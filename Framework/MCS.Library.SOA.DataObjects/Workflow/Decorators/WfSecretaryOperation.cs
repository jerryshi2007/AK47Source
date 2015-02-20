using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public enum WfSecretaryOperationType
	{
		/// <summary>
		/// 无操作
		/// </summary>
		None,

		/// <summary>
		/// 增加秘书活动
		/// </summary>
		AddSecretaries,

		/// <summary>
		/// 修改秘书活动
		/// </summary>
		ChangeSecretaries,

		/// <summary>
		/// 清除秘书活动
		/// </summary>
		ClearSecretaries
	}

	/// <summary>
	/// 经过判断后需要处理的秘书活动相关操作
	/// </summary>
	public class WfSecretaryOperation
	{
		public WfSecretaryOperationType OperationType
		{
			get;
			set;
		}

		public IEnumerable<IUser> Secretaries
		{
			get;
			set;
		}

		public IWfActivityDescriptor ActivityDescriptor
		{
			get;
			set;
		}
	}
}
