using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 数据对象的状态（主要针对逻辑删除问题[回收站问题]），用于构造查询筛选器
	/// </summary>
	[Flags]
	public enum ListObjectDelete : int
	{
		/// <summary>
		/// 非法筛选
		/// </summary>
		[EnumItemDescription("非法筛选")]
		None = 0,
		/// <summary>
		/// 筛选器中要求查询正常使用的数据对象
		/// </summary>
		[EnumItemDescription("正常使用的数据对象")]
		COMMON = 1,
		/// <summary>
		/// 筛选器中要求查询“直接逻辑删除对象”
		/// </summary>
		[EnumItemDescription("直接逻辑删除对象")]
		DIRECT_LOGIC = 2,
		/// <summary>
		/// 筛选器中要求查询“因部门导致数据逻辑删除对象”
		/// </summary>
		[EnumItemDescription("因部门导致数据逻辑删除对象")]
		CONJUNCT_ORG_LOGIC = 4,
		/// <summary>
		/// 筛选器中要求查询“因人员导致数据逻辑删除对象”
		/// </summary>
		[EnumItemDescription("因人员导致数据逻辑删除对象")]
		CONJUNCT_USER_LOGIC = 8,
		/// <summary>
		/// 系统中所有的数据对象
		/// </summary>
		[EnumItemDescription("所有的数据对象")]
		ALL_TYPE = 65535
	}
}
