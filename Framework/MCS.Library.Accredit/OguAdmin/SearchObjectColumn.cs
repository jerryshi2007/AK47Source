using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ���ݲ�ѯ����
	/// </summary>
	[Flags]
	public enum SearchObjectColumn : int
	{
		/// <summary>
		/// �ղ�ѯ
		/// </summary>
		[EnumItemDescription("�ղ�ѯ")]
		SEARCH_NULL = 0,
		/// <summary>
		/// ��������GUID��ѯ
		/// </summary>
		[EnumItemDescription("����GUID��ѯ")]
		SEARCH_GUID = 1,
		/// <summary>
		/// ��������USER_GUID��ѯ
		/// </summary>
		[EnumItemDescription("��������USER_GUID��ѯ")]
		SEARCH_USER_GUID = 2,
		/// <summary>
		/// ��������ORIGINAL_SORT��ѯ
		/// </summary>
		[EnumItemDescription("��������ORIGINAL_SORT��ѯ")]
		SEARCH_ORIGINAL_SORT = 3,
		/// <summary>
		/// ��������GLOBAL_SORT��ѯ
		/// </summary>
		[EnumItemDescription("��������GLOBAL_SORT��ѯ")]
		SEARCH_GLOBAL_SORT = 4,
		/// <summary>
		/// ��������ALL_PATH_NAME��ѯ
		/// </summary>
		[EnumItemDescription("��������ALL_PATH_NAME��ѯ")]
		SEARCH_ALL_PATH_NAME = 5,
		/// <summary>
		/// ��������LOGON_NAME��ѯ
		/// </summary>
		[EnumItemDescription("��������LOGON_NAME��ѯ")]
		SEARCH_LOGON_NAME = 6,
		/// <summary>
		/// ����PERSON_ID(������Ա���)��ѯ
		/// </summary>
		[EnumItemDescription("����PERSON_ID(������Ա���)��ѯ")]
		SEARCH_PERSON_ID = 7,
		/// <summary>
		/// ����IC_CARD��ѯ
		/// </summary>
		[EnumItemDescription("����IC_CARD��ѯ")]
		SEARCH_IC_CARD = 8,
		/// <summary>
		/// ����CUSTOMS_CODE��ѯ
		/// </summary>
		[EnumItemDescription("����CUSTOMS_CODE��ѯ")]
		SEARCH_CUSTOMS_CODE = 9,
		/// <summary>
		/// ����Ψһ������ѯ��ORGANIZATIONS\GROUPS\USERS�����ֶ�1��
		/// </summary>
		[EnumItemDescription("����Ψһ����SYSDISTINCT1��ѯ")]
		SEARCH_SYSDISTINCT1 = 16,
		/// <summary>
		/// ����Ψһ������ѯ��ORGANIZATIONS\GROUPS\USERS�����ֶ�2��
		/// </summary>
		[EnumItemDescription("����Ψһ����SYSDISTINCT2��ѯ")]
		SEARCH_SYSDISTINCT2 = 32,
		/// <summary>
		/// ����Ψһ������ѯ��OU_USERS�����ֶ�1��
		/// </summary>
		[EnumItemDescription("����Ψһ����OUSYSDISTINCT1��ѯ")]
		SEARCH_OUSYSDISTINCT1 = 64,
		/// <summary>
		/// ����Ψһ������ѯ��OU_USERS�����ֶ�2��
		/// </summary>
		[EnumItemDescription("����Ψһ����OUSYSDISTINCT2��ѯ")]
		SEARCH_OUSYSDISTINCT2 = 128,
		/// <summary>
		/// ����Ψһ������ѯ(Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�])
		/// </summary>
		SEARCH_IDENTITY = 256
	}
}
