using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// ְλ����
	/// </summary>
	[Flags]
	public enum SidelineMaskType
	{
		/// <summary>
		/// �Ƿ�ְ��
		/// </summary>
		None = 0,
		/// <summary>
		/// ��ְ
		/// </summary>
		NotSideline = 1,
		/// <summary>
		/// ��ְ
		/// </summary>
		Sideline = 2,
		/// <summary>
		/// ȫ������ְ����ְ��
		/// </summary>
		All = 3
	}
}
