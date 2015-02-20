using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 任务流程的分支流程的阻塞类型
	/// </summary>
	public enum SysTaskProcessBlockingType
	{
		/// <summary>
		/// 等待全部分支流程完成
		/// </summary>
		[EnumItemDescription("等待全部分支流程完成")]
		WaitAllBranchProcessesComplete,

		/// <summary>
		/// 任何分支流程都不等待
		/// </summary>
		[EnumItemDescription("任何分支流程都不等待")]
		WaitNoneOfBranchProcessComplete,

		/// <summary>
		/// 等待任意一个分支流程完成
		/// </summary>
		[EnumItemDescription("等待任意一个分支流程完成")]
		WaitAnyoneBranchProcessComplete
	}
}
