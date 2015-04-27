using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
	/// <summary>
	/// ��¼��Ϣ�Ľӿ�
	/// </summary>
	public interface ISignInInfo
	{
		/// <summary>
		/// ��¼�û���ID
		/// </summary>
		string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// ����ǰ�ĵ�¼��
		/// </summary>
		string OriginalUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		string Domain
		{
			get;
		}

		/// <summary>
		/// ���������û�ID
		/// </summary>
		string UserIDWithDomain
		{
			get;
		}

		/// <summary>
		/// �Ƿ񼯳���֤
		/// </summary>
		bool WindowsIntegrated
		{
			get;
		}

		/// <summary>
		/// ��¼��SessionID
		/// </summary>
		string SignInSessionID
		{
			get;
		}

		/// <summary>
		/// ��¼��ʱ��
		/// </summary>
 		DateTime SignInTime
		{
			get;
			set;
		}

		/// <summary>
		/// ��¼�Ĺ���ʱ��
		/// </summary>
		DateTime SignInTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// ��֤������������(����IP)
		/// </summary>
		string AuthenticateServer
		{
			get;
		}

		/// <summary>
		/// �Ƿ���ڵ�¼��ʱʱ�䣨�������ڵ�������Сֵ��
		/// </summary>
		bool ExistsSignInTimeout
		{
			get;
		}

        /// <summary>
        /// �⻧����
        /// </summary>
        string TenantCode
        {
            get;
            set;
        }

		/// <summary>
		/// ��չ���Լ��ϣ�����⣩
		/// </summary>
		Dictionary<string, object> Properties
		{
			get;
		}

		/// <summary>
		/// ����¼��Ϣ���浽Cookie��
		/// </summary>
		void SaveToCookie();

		/// <summary>
		/// ����¼��Ϣ���浽Xml�ĵ�������
		/// </summary>
		XmlDocument SaveToXml();

		/// <summary>
		/// SignInInfo�Ƿ�Ϸ�
		/// </summary>
		/// <returns></returns>
		bool IsValid();
	}

	/// <summary>
	/// Ӧ�õ�¼�Ժ����ɵ�Ticket
	/// </summary>
	public interface ITicket
	{
		/// <summary>
		/// ��¼����Ϣ
		/// </summary>
		//[NoMapping]
		ISignInInfo SignInInfo
		{
			get;
		}

		/// <summary>
		/// Ӧ�õ�¼�Ժ��Ӧ��ID
		/// </summary>
        string AppSignInSessionID
		{
			get;
		}

		/// <summary>
		/// Ӧ�õ�ID
		/// </summary>
		string AppID
		{
			get;
		}

		/// <summary>
		/// Ӧ�õ�¼��ʱ��
		/// </summary>
		DateTime AppSignInTime
		{
			get;
			set;
		}

		/// <summary>
		/// Ӧ�õ�¼��Session����ʱ��
		/// </summary>
		DateTime AppSignInTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// Ӧ�õ�¼ʱ��IP��ַ
		/// </summary>
		string AppSignInIP
		{
			get;
		}

		/// <summary>
		/// ��Ӧ�õ�¼��Ϣ���浽Cookie��
		/// </summary>
		void SaveToCookie();

		/// <summary>
		/// ��Ӧ�õ�¼��Ϣ���浽Xml�ĵ�������
		/// </summary>
		XmlDocument SaveToXml();

		/// <summary>
		/// Ticket�Ƿ�Ϸ�
		/// </summary>
		/// <returns></returns>
		bool IsValid();

		/// <summary>
		/// ���ɼ��ܵ��ַ���
		/// </summary>
		/// <returns></returns>
		string ToEncryptString();
	}

	/// <summary>
	/// ��¼�û�����Ϣ
	/// </summary>
	public interface ISignInUserInfo
	{
		/// <summary>
		/// ��¼�û���ID
		/// </summary>
		string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// �û�������
		/// </summary>
		string Domain
		{
			get;
			set;
		}

		/// <summary>
		/// ԭʼ�ĵ�¼ID
		/// </summary>
		string OriginalUserID
		{
			get;
			set;
		}

		/// <summary>
		/// ���Լ���
		/// </summary>
		Dictionary<string, object> Properties
		{
			get;
		}
	}
}
