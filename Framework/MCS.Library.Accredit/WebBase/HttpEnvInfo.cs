using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using MCS.Library.Accredit.WebBase.Interfaces;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// HttpEnvInfo ��ժҪ˵����
	/// </summary>
	class HttpEnvInfo : IHttpEnvInterface
	{
		/// <summary>
		/// ��ǰ�����еĻ�����Ϣ����
		/// </summary>
		/// <param name="request">���淢�͹�������������</param>
		public HttpEnvInfo(HttpRequest request)
		{
			_Url = request.Url;
			_UserAgent = request.UserAgent;
			_UserHostAddress = request.UserHostAddress;
			_UserHostName = request.UserHostName;
		}


		#region IHttpEnvInterface ��Ա

		private Uri _Url = null;
		/// <summary>
		/// �ͻ��˷��������IP��ַ
		/// </summary>
		public Uri Url
		{
			get
			{
				return _Url;
			}
		}

		private string _UserHostName = string.Empty;
		/// <summary>
		/// �ͻ��˷�������Ļ�����
		/// </summary>
		public string UserHostName
		{
			get
			{
				return _UserHostName;
			}
		}

		private string _UserAgent = string.Empty;
		/// <summary>
		/// �ͻ��˷���������������UserAgent
		/// </summary>
		public string UserAgent
		{
			get
			{
				return _UserAgent;
			}
		}

		private string _UserHostAddress = string.Empty;
		/// <summary>
		/// ��ҳ�������URL
		/// </summary>
		public string UserHostAddress
		{
			get
			{
				return _UserHostAddress;
			}
		}

		#endregion
	}
}
