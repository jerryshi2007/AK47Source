using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 数据查询条件
	/// </summary>
	[Flags]
	public enum SearchObjectColumn : int
	{
		/// <summary>
		/// 空查询
		/// </summary>
		[EnumItemDescription("空查询")]
		SEARCH_NULL = 0,
		/// <summary>
		/// 根据数据GUID查询
		/// </summary>
		[EnumItemDescription("根据GUID查询")]
		SEARCH_GUID = 1,
		/// <summary>
		/// 根据数据USER_GUID查询
		/// </summary>
		[EnumItemDescription("根据数据USER_GUID查询")]
		SEARCH_USER_GUID = 2,
		/// <summary>
		/// 根据数据ORIGINAL_SORT查询
		/// </summary>
		[EnumItemDescription("根据数据ORIGINAL_SORT查询")]
		SEARCH_ORIGINAL_SORT = 3,
		/// <summary>
		/// 根据数据GLOBAL_SORT查询
		/// </summary>
		[EnumItemDescription("根据数据GLOBAL_SORT查询")]
		SEARCH_GLOBAL_SORT = 4,
		/// <summary>
		/// 根据数据ALL_PATH_NAME查询
		/// </summary>
		[EnumItemDescription("根据数据ALL_PATH_NAME查询")]
		SEARCH_ALL_PATH_NAME = 5,
		/// <summary>
		/// 根据数据LOGON_NAME查询
		/// </summary>
		[EnumItemDescription("根据数据LOGON_NAME查询")]
		SEARCH_LOGON_NAME = 6,
		/// <summary>
		/// 根据PERSON_ID(海关人员编号)查询
		/// </summary>
		[EnumItemDescription("根据PERSON_ID(海关人员编号)查询")]
		SEARCH_PERSON_ID = 7,
		/// <summary>
		/// 根据IC_CARD查询
		/// </summary>
		[EnumItemDescription("根据IC_CARD查询")]
		SEARCH_IC_CARD = 8,
		/// <summary>
		/// 根据CUSTOMS_CODE查询
		/// </summary>
		[EnumItemDescription("根据CUSTOMS_CODE查询")]
		SEARCH_CUSTOMS_CODE = 9,
		/// <summary>
		/// 根据唯一索引查询（ORGANIZATIONS\GROUPS\USERS备用字段1）
		/// </summary>
		[EnumItemDescription("根据唯一索引SYSDISTINCT1查询")]
		SEARCH_SYSDISTINCT1 = 16,
		/// <summary>
		/// 根据唯一索引查询（ORGANIZATIONS\GROUPS\USERS备用字段2）
		/// </summary>
		[EnumItemDescription("根据唯一索引SYSDISTINCT2查询")]
		SEARCH_SYSDISTINCT2 = 32,
		/// <summary>
		/// 根据唯一索引查询（OU_USERS备用字段1）
		/// </summary>
		[EnumItemDescription("根据唯一索引OUSYSDISTINCT1查询")]
		SEARCH_OUSYSDISTINCT1 = 64,
		/// <summary>
		/// 根据唯一索引查询（OU_USERS备用字段2）
		/// </summary>
		[EnumItemDescription("根据唯一索引OUSYSDISTINCT2查询")]
		SEARCH_OUSYSDISTINCT2 = 128,
		/// <summary>
		/// 根据唯一索引查询(为配合南京海关统一平台切换，新增加字段ID[自增唯一字段])
		/// </summary>
		SEARCH_IDENTITY = 256
	}
}
