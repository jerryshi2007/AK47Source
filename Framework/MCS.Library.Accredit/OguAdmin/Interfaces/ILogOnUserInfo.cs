using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// ILogOnUserInfo ��ժҪ˵����
	/// </summary>
	public interface ILogOnUserInfo : IPrincipal
	{
		/// <summary>
		/// �û������Ӧ�ı�ʶ
		/// </summary>
		string UserGuid
		{
			get;
		}

		/// <summary>
		/// �û��ĵ�¼��
		/// </summary>
		string UserLogOnName
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�����������
		/// </summary>
		IRankDefine RankDefine
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û������ڵ����п��õĻ�����Ա��ϵ
		/// </summary>
		IOuUsers[] OuUsers
		{
			get;
		}
	}
}
