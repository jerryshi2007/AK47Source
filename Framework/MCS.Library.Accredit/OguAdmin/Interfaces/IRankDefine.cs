using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// IRankDefine ��ժҪ˵����
	/// </summary>
	public interface IRankDefine
	{
		#region �������Զ���

		/// <summary>
		/// �����������Ӧ��Ӣ�ı�ʶ
		/// </summary>
		string CodeName
		{
			get;
		}

		/// <summary>
		/// ����������Ĵ��򣨸߼���ʹ���
		/// </summary>
		int SortID
		{
			get;
		}

		/// <summary>
		/// ���������������
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// �����������Ƿ�ɼ�
		/// </summary>
		bool Visible
		{
			get;
		}

		#endregion
	}
}
