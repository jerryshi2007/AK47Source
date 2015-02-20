#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	FunctionNames.cs
// Remark	��		����Χö�ٶ��壬�����˶����Ʒ�ʽ���뷽ʽʵ��
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		2008121630		�´���
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	///	����Χ���� �������˶����Ʒ�ʽ���뷽ʽʵ��
	/// </summary>
	[Flags]
	public enum ScopeMaskType
	{
		/// <summary>
		/// �Ƿ���Χ
		/// </summary>
		/// <remarks>�Ƿ���Χ</remarks>
		None = 0,
		/// <summary>
		/// ��������Χ
		/// </summary>
		/// <remarks>��������Χ</remarks>
		OrgScope = 1,
		/// <summary>
		/// ���ݷ���Χ
		/// </summary>
		/// <remarks>���ݷ���Χ</remarks>
		DataScope = 2,
		/// <summary>
		/// ȫ������������Χ�����ݷ���Χ��
		/// </summary>
		/// <remarks>ȫ������������Χ�����ݷ���Χ��</remarks>
		All = 3
	}
}
