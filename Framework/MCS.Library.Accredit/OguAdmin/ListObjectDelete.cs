using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ���ݶ����״̬����Ҫ����߼�ɾ������[����վ����]�������ڹ����ѯɸѡ��
	/// </summary>
	[Flags]
	public enum ListObjectDelete : int
	{
		/// <summary>
		/// �Ƿ�ɸѡ
		/// </summary>
		[EnumItemDescription("�Ƿ�ɸѡ")]
		None = 0,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ����ʹ�õ����ݶ���
		/// </summary>
		[EnumItemDescription("����ʹ�õ����ݶ���")]
		COMMON = 1,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ��ֱ���߼�ɾ������
		/// </summary>
		[EnumItemDescription("ֱ���߼�ɾ������")]
		DIRECT_LOGIC = 2,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ�����ŵ��������߼�ɾ������
		/// </summary>
		[EnumItemDescription("���ŵ��������߼�ɾ������")]
		CONJUNCT_ORG_LOGIC = 4,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ������Ա���������߼�ɾ������
		/// </summary>
		[EnumItemDescription("����Ա���������߼�ɾ������")]
		CONJUNCT_USER_LOGIC = 8,
		/// <summary>
		/// ϵͳ�����е����ݶ���
		/// </summary>
		[EnumItemDescription("���е����ݶ���")]
		ALL_TYPE = 65535
	}
}
