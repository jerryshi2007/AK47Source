#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	FunctionNames.cs
// Remark	：		权限类型 的 枚举定义，采用了二进制方式掩码方式实现
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
	/// 权限类型
	/// </summary>
	/// <remarks>系统中使用权限的枚举定义，采用了二进制方式掩码方式实现</remarks>
	[Flags]
	public enum RightMaskType
	{
		/// <summary>
		/// 非法授权
		/// </summary>
		/// <remarks>非法授权</remarks>
		None = 0,
		/// <summary>
		/// 自授权
		/// </summary>
		/// <remarks>自授权</remarks>
		Self = 1,
		/// <summary>
		/// 应用授权
		/// </summary>
		/// <remarks>应用授权</remarks>
		App = 2,
		/// <summary>
		/// 全部（自授权、应用授权）
		/// </summary>
		/// <remarks>全部（自授权、应用授权）</remarks>
		All = 3
	}
}
