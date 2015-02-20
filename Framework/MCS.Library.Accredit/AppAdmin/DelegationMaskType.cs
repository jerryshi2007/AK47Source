#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	DelegationMaskType.cs
// Remark	��		ϵͳö�ٶ���ί�����͡��Ķ��壬�����˶����Ƶ�����ʵ�ַ�ʽ
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
	/// ί������
	/// </summary>
	/// <remarks>
	/// �û�Ȩ��ί�ɵ���������
	/// </remarks>
	[Flags]
	public enum DelegationMaskType
	{
		/// <summary>
		/// �Ƿ�ί��
		/// </summary>
		/// <remarks>
		/// ���������ݣ����ö����Ƶ����뷽ʽ����ʵ��
		/// </remarks>
		None = 0,
		/// <summary>
		/// ����ԭʼȨ��
		/// </summary>
		/// <remarks>
		/// �������� �û�ԭʼȨ��
		/// </remarks>
		Original = 1,
		/// <summary>
		/// ���鱻ί��Ȩ��
		/// </summary>
		/// <remarks>
		/// ���鱻ί��Ȩ��
		/// </remarks>
		Delegated = 2,
		/// <summary>
		/// ��ѯԭʼ�ͱ�ί�ɵ�Ȩ���ۺ�
		/// </summary>
		/// <remarks>
		/// ��ѯԭʼ�ͱ�ί�ɵ�Ȩ���ۺ�
		/// </remarks>
		All = 3
	}
}
