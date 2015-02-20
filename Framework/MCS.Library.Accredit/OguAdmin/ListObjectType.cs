using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 数据对象类型，一般用于构造查询筛选器
	/// </summary>
	[Flags]
	public enum ListObjectType : int
	{
		/// <summary>
		/// 非法筛选器
		/// </summary>
		[EnumItemDescription("非法对象")]
		None = 0,
		/// <summary>
		/// 筛选其中要求查询“机构对象”
		/// </summary>
		[EnumItemDescription("机构")]
		ORGANIZATIONS = 1,
		/// <summary>
		/// 筛选其中要求查询“人员对象”
		/// </summary>
		[EnumItemDescription("人员")]
		USERS = 2,
		/// <summary>
		/// 筛选器中要求查询“人员组对象”
		/// </summary>
		[EnumItemDescription("人员组")]
		GROUPS = 4,
		/// <summary>
		/// 筛选器中要求查询“人员兼职对象”
		/// </summary>
		[EnumItemDescription("兼职")]
		SIDELINE = 8,
		/// <summary>
		/// 筛选器中所能允许的所有数据对象
		/// </summary>
		[EnumItemDescription("所有类型")]
		ALL_TYPE = 65535
	}
}
