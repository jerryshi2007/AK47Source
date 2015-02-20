using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ���ݶ������ͣ�һ�����ڹ����ѯɸѡ��
	/// </summary>
	[Flags]
	public enum ListObjectType : int
	{
		/// <summary>
		/// �Ƿ�ɸѡ��
		/// </summary>
		[EnumItemDescription("�Ƿ�����")]
		None = 0,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ����������
		/// </summary>
		[EnumItemDescription("����")]
		ORGANIZATIONS = 1,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ����Ա����
		/// </summary>
		[EnumItemDescription("��Ա")]
		USERS = 2,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ����Ա�����
		/// </summary>
		[EnumItemDescription("��Ա��")]
		GROUPS = 4,
		/// <summary>
		/// ɸѡ����Ҫ���ѯ����Ա��ְ����
		/// </summary>
		[EnumItemDescription("��ְ")]
		SIDELINE = 8,
		/// <summary>
		/// ɸѡ��������������������ݶ���
		/// </summary>
		[EnumItemDescription("��������")]
		ALL_TYPE = 65535
	}
}
