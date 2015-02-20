using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// IRankDefine 的摘要说明。
	/// </summary>
	public interface IRankDefine
	{
		#region 数据属性定义

		/// <summary>
		/// 该行政级别对应的英文标识
		/// </summary>
		string CodeName
		{
			get;
		}

		/// <summary>
		/// 该行政级别的次序（高级别低次序）
		/// </summary>
		int SortID
		{
			get;
		}

		/// <summary>
		/// 该行政级别的名称
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 该行政级别是否可见
		/// </summary>
		bool Visible
		{
			get;
		}

		#endregion
	}
}
