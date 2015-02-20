using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.WebBase.Interfaces
{
	/// <summary>
	/// ��Http����������صĽӿ�
	/// </summary>
	public interface IHttpEnvInterface
	{
		/// <summary>
		/// �ͻ��˷��������IP��ַ
		/// </summary>
		string UserHostAddress
		{
			get;
		}

		/// <summary>
		/// �ͻ��˷�������Ļ�����
		/// </summary>
		string UserHostName
		{
			get;
		}

		/// <summary>
		/// �ͻ��˷���������������UserAgent
		/// </summary>
		string UserAgent
		{
			get;
		}

		/// <summary>
		/// ��ҳ�������URL
		/// </summary>
		Uri Url
		{
			get;
		}
	}
}
