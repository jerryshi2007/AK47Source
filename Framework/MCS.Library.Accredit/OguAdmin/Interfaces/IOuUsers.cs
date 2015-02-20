using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// IOuUser 的摘要说明。
	/// </summary>
	public interface IOuUsers
	{
		/// <summary>
		/// 当前用户机构关系中用户对应的标识
		/// </summary>
		string UserGuid
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中机构对应的标识
		/// </summary>
		string OUGuid
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中该用户的“显示名称”
		/// </summary>
		string UserDisplayName
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中用户的“对象名称”（考虑一个部门中可能出现因为对象名称造成的数据冲突）
		/// </summary>
		string UserObjName
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中用户在该机构中的排序（内部排序）
		/// </summary>
		string InnerSort
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中用户的全地址（关于层次上的排序）
		/// </summary>
		string GlobalSort
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中用户的全地址（层次关系描述，不用于排序）
		/// </summary>
		string OriginalSort
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系中用户的全地址表示（文字描述）
		/// </summary>
		string AllPathName
		{
			get;
		}

		/// <summary>
		/// 当前用户的附加描述信息
		/// </summary>
		string UserDescription
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系是否兼职关系（兼职：true；主职：false）
		/// </summary>
		bool Sideline
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系的启用时间
		/// </summary>
		DateTime StartTime
		{
			get;
		}

		/// <summary>
		/// 当前用户机构关系的结束时间
		/// </summary>
		DateTime EndTime
		{
			get;
		}

		/// <summary>
		/// 根据启用时间和结束时间判断当前用户机构关系是否生效
		/// </summary>
		bool InUse
		{
			get;
		}
	}
}
