#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	FunctionNames.cs
// Remark	��		Ȩ������ �� ö�ٶ��壬�����˶����Ʒ�ʽ���뷽ʽʵ��
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
	/// Ȩ������
	/// </summary>
	/// <remarks>ϵͳ��ʹ��Ȩ�޵�ö�ٶ��壬�����˶����Ʒ�ʽ���뷽ʽʵ��</remarks>
	[Flags]
	public enum RightMaskType
	{
		/// <summary>
		/// �Ƿ���Ȩ
		/// </summary>
		/// <remarks>�Ƿ���Ȩ</remarks>
		None = 0,
		/// <summary>
		/// ����Ȩ
		/// </summary>
		/// <remarks>����Ȩ</remarks>
		Self = 1,
		/// <summary>
		/// Ӧ����Ȩ
		/// </summary>
		/// <remarks>Ӧ����Ȩ</remarks>
		App = 2,
		/// <summary>
		/// ȫ��������Ȩ��Ӧ����Ȩ��
		/// </summary>
		/// <remarks>ȫ��������Ȩ��Ӧ����Ȩ��</remarks>
		All = 3
	}
}
