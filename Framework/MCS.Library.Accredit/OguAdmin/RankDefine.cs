using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 行政级别类
	/// </summary>
	public class RankDefine : Interfaces.IRankDefine
	{
		/// <summary>
		/// 构造函数（初始化一项行政级别）
		/// </summary>
		/// <param name="strCodeName">英文标志名称</param>
		/// <param name="iSortID">内部排序号</param>
		/// <param name="strName">显示名称</param>
		/// <param name="iVisible">是否要求展现（0：不可见；1：可见）</param>
		public RankDefine(string strCodeName, int iSortID, string strName, int iVisible)
		{
			_StrCodeName = strCodeName;
			_StrName = strName;
			_ISortID = iSortID;

			if (iVisible == 1)
				_BVisible = true;
		}

		private string _StrCodeName = string.Empty;
		/// <summary>
		/// 该行政级别对应的英文标识
		/// </summary>
		public string CodeName
		{
			get
			{
				return _StrCodeName;
			}
		}

		private int _ISortID = 0;
		/// <summary>
		/// 该行政级别的次序（高级别低次序）
		/// </summary>
		public int SortID
		{
			get
			{
				return _ISortID;
			}
		}

		private string _StrName = string.Empty;
		/// <summary>
		/// 该行政级别的名称
		/// </summary>
		public string Name
		{

			get
			{
				return _StrName;
			}
		}

		private bool _BVisible = false;
		/// <summary>
		/// 该行政级别是否可见
		/// </summary>
		public bool Visible
		{
			get
			{
				return _BVisible;
			}
		}
	}
}
