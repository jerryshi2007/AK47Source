using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// ��Ա��ʶ����
	/// </summary>
	public enum UserValueType
	{
		/// <summary>
		/// ��¼��
		/// </summary>
		LogonName = 1,
		/// <summary>
		/// ��Աȫ·��
		/// </summary>
		AllPath = 2,
		/// <summary>
		/// ��Ա���
		/// </summary>
		PersonID = 3,
		/// <summary>
		/// IC����
		/// </summary>
		ICCode = 4,
		/// <summary>
		/// ��ԱGuidֵ
		/// </summary>
		Guid = 8,
		/// <summary>
		/// ����Ψһ������ѯ(Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�])
		/// </summary>
		Identity = 16
	}
}
