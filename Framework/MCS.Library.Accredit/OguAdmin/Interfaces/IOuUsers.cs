using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// IOuUser ��ժҪ˵����
	/// </summary>
	public interface IOuUsers
	{
		/// <summary>
		/// ��ǰ�û�������ϵ���û���Ӧ�ı�ʶ
		/// </summary>
		string UserGuid
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ�л�����Ӧ�ı�ʶ
		/// </summary>
		string OUGuid
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ�и��û��ġ���ʾ���ơ�
		/// </summary>
		string UserDisplayName
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ���û��ġ��������ơ�������һ�������п��ܳ�����Ϊ����������ɵ����ݳ�ͻ��
		/// </summary>
		string UserObjName
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ���û��ڸû����е������ڲ�����
		/// </summary>
		string InnerSort
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ�����ڲ���ϵ�����
		/// </summary>
		string GlobalSort
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ����ι�ϵ����������������
		/// </summary>
		string OriginalSort
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ��ʾ������������
		/// </summary>
		string AllPathName
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û��ĸ���������Ϣ
		/// </summary>
		string UserDescription
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ�Ƿ��ְ��ϵ����ְ��true����ְ��false��
		/// </summary>
		bool Sideline
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ������ʱ��
		/// </summary>
		DateTime StartTime
		{
			get;
		}

		/// <summary>
		/// ��ǰ�û�������ϵ�Ľ���ʱ��
		/// </summary>
		DateTime EndTime
		{
			get;
		}

		/// <summary>
		/// ��������ʱ��ͽ���ʱ���жϵ�ǰ�û�������ϵ�Ƿ���Ч
		/// </summary>
		bool InUse
		{
			get;
		}
	}
}
