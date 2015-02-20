using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Web;
using System.Xml;

using MCS.Library.Core;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// �쳣������
	/// </summary>
	class TransExceptionInfo
	{
		private string _Message = string.Empty;
		private string _StackTrace = string.Empty;
		private string _SupportHtml = string.Empty;
		private Exception _Exception = null;

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="ex"></param>
		public TransExceptionInfo(System.Exception ex)
		{
			string strMsg = CommonDefine.GetSoapExceptionMessage(ex);

			if (ex is SqlException)
			{
				SqlException sqlException = (SqlException)ex;

				if (sqlException.Number == 7619)
					strMsg = "�Բ���������Ĳ�ѯ���к����ķ��ţ�����ȫ�ļ�������������⡣";
			}
			else
				if (ex is System.TypeInitializationException || ex is System.Reflection.TargetInvocationException)
				{
					ex = ex.InnerException;
					strMsg = HttpUtility.HtmlEncode(ex.Message);
				}

			_Message = strMsg;
			_StackTrace = ex.StackTrace;
			_Exception = ex;
		}

		/// <summary>
		/// ������Ϣ
		/// </summary>
		public string Message
		{
			get
			{
				return _Message;
			}
		}

		/// <summary>
		/// ��ջ��Ϣ
		/// </summary>
		public string StackTrace
		{
			get
			{
				return _StackTrace;
			}
		}

		/// <summary>
		/// Ҫ��չ�ֵ�HTML
		/// </summary>
		public string SupportHtml
		{
			get
			{
				return _SupportHtml;
			}
		}

		/// <summary>
		/// �쳣����
		/// </summary>
		public System.Exception Exception
		{
			get
			{
				return _Exception;
			}
		}

		///// <summary>
		///// �������ļ��еõ������ⷴ����Ϣ������������
		///// </summary>
		///// <returns>�����ⷴ����Ϣ������������</returns>
		//public static string GetSupportMessage()
		//{
		//    string strResult = string.Empty;

		//    SysConfig sc = new SysConfig();

		//    XmlNode node = sc.GetSectionNode("BaseWebClass", "SupportMessage");

		//    if (node != null)
		//        strResult = node.InnerXml.Trim();

		//    return strResult;
		//}
	}
}
