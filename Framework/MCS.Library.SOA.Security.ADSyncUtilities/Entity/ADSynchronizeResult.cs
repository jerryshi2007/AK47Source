using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public enum ADSynchronizeResult
	{
		/// <summary>
		/// 正确
		/// </summary>
		[EnumItemDescription("正确")]
		Correct,

		/// <summary>
		/// 有错误
		/// </summary>
		[EnumItemDescription("有错误")]
		HasError,

		/// <summary>
		/// 异常中止
		/// </summary>
		[EnumItemDescription("异常中止")]
		Interrupted,

		/// <summary>
		/// 执行中，未结束
		/// </summary>
		[EnumItemDescription("执行中")]
		Running
	}
}
