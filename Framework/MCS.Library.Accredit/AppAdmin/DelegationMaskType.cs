#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	DelegationMaskType.cs
// Remark	：		系统枚举对象“委派类型”的定义，采用了二进制的掩码实现方式
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		新创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// 委派类型
	/// </summary>
	/// <remarks>
	/// 用户权限委派的数据类型
	/// </remarks>
	[Flags]
	public enum DelegationMaskType
	{
		/// <summary>
		/// 非法委派
		/// </summary>
		/// <remarks>
		/// 非正常数据，采用二进制的掩码方式进行实现
		/// </remarks>
		None = 0,
		/// <summary>
		/// 仅查原始权限
		/// </summary>
		/// <remarks>
		/// 这里仅针对 用户原始权限
		/// </remarks>
		Original = 1,
		/// <summary>
		/// 仅查被委派权限
		/// </summary>
		/// <remarks>
		/// 仅查被委派权限
		/// </remarks>
		Delegated = 2,
		/// <summary>
		/// 查询原始和被委派的权限综合
		/// </summary>
		/// <remarks>
		/// 查询原始和被委派的权限综合
		/// </remarks>
		All = 3
	}
}
