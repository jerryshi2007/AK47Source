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
	/// 异常解析器
	/// </summary>
	class TransExceptionInfo
	{
		private string _Message = string.Empty;
		private string _StackTrace = string.Empty;
		private string _SupportHtml = string.Empty;
		private Exception _Exception = null;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="ex"></param>
		public TransExceptionInfo(System.Exception ex)
		{
			string strMsg = CommonDefine.GetSoapExceptionMessage(ex);

			if (ex is SqlException)
			{
				SqlException sqlException = (SqlException)ex;

				if (sqlException.Number == 7619)
					strMsg = "对不起！你输入的查询中有含糊的符号，或者全文检索服务出现问题。";
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
		/// 描述信息
		/// </summary>
		public string Message
		{
			get
			{
				return _Message;
			}
		}

		/// <summary>
		/// 堆栈信息
		/// </summary>
		public string StackTrace
		{
			get
			{
				return _StackTrace;
			}
		}

		/// <summary>
		/// 要求展现的HTML
		/// </summary>
		public string SupportHtml
		{
			get
			{
				return _SupportHtml;
			}
		}

		/// <summary>
		/// 异常本身
		/// </summary>
		public System.Exception Exception
		{
			get
			{
				return _Exception;
			}
		}

		///// <summary>
		///// 从配置文件中得到“问题反馈信息”的文字描述
		///// </summary>
		///// <returns>“问题反馈信息”的文字描述</returns>
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
