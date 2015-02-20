using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 任务流程活动的状态
	/// </summary>
	public enum SysTaskActivityStatus
	{
		/// <summary>
		/// 未运行
		/// </summary>
		[EnumItemDescription("未运行")]
		NotRunning = 0,

		/// <summary>
		/// 运行中
		/// </summary>
		[EnumItemDescription("运行中")]
		Running,

		/// <summary>
		/// 已完成
		/// </summary>
		[EnumItemDescription("已完成")]
		Completed,

		/// <summary>
		/// 已中止
		/// </summary>
		[EnumItemDescription("已中止")]
		Aborted,

		/// <summary>
		/// 已挂起
		/// </summary>
		[EnumItemDescription("已挂起")]
		Pending
	}
}
