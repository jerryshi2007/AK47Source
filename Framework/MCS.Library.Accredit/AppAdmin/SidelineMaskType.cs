using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// 职位类型
	/// </summary>
	[Flags]
	public enum SidelineMaskType
	{
		/// <summary>
		/// 非法职务
		/// </summary>
		None = 0,
		/// <summary>
		/// 正职
		/// </summary>
		NotSideline = 1,
		/// <summary>
		/// 兼职
		/// </summary>
		Sideline = 2,
		/// <summary>
		/// 全部（正职、兼职）
		/// </summary>
		All = 3
	}
}
