#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	FunctionNames.cs
// Remark	：		服务范围枚举定义，采用了二进制方式掩码方式实现
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
	///	服务范围类型 ，采用了二进制方式掩码方式实现
	/// </summary>
	[Flags]
	public enum ScopeMaskType
	{
		/// <summary>
		/// 非法范围
		/// </summary>
		/// <remarks>非法范围</remarks>
		None = 0,
		/// <summary>
		/// 机构服务范围
		/// </summary>
		/// <remarks>机构服务范围</remarks>
		OrgScope = 1,
		/// <summary>
		/// 数据服务范围
		/// </summary>
		/// <remarks>数据服务范围</remarks>
		DataScope = 2,
		/// <summary>
		/// 全部（机构服务范围、数据服务范围）
		/// </summary>
		/// <remarks>全部（机构服务范围、数据服务范围）</remarks>
		All = 3
	}
}
