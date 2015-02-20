#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ExceptionHelper.cs
// Remark	��	Exception���ߣ�TrueThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊtrue���������׳��쳣��FalseThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊfalse���������׳��쳣�� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.Contracts
{
	/// <summary>
	/// Exception���ߣ��ṩ��TrueThrow��FalseThrow�ȷ���
	/// </summary>
	/// <remarks>Exception���ߣ�TrueThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊtrue���������׳��쳣��FalseThrow�����ж����Ĳ�������ֵ�Ƿ�Ϊfalse���������׳��쳣��
	/// </remarks>
	internal static class ExceptionHelper
	{
		/// <summary>
		/// �������Ƿ�Ϊ�գ����Ϊ�գ��׳�ArgumentNullException
		/// </summary>
		/// <param name="data">�����Ķ���</param>
		/// <param name="message">����������</param>
		public static void NullCheck(this object data, string message)
		{
			NullCheck<ArgumentNullException>(data, message);
		}
		
		/// <summary>
		/// �������Ƿ�Ϊ�գ����Ϊ�գ��׳��쳣
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="message"></param>
		/// <param name="messageParams"></param>
		public static void NullCheck<T>(this object data, string message, params object[] messageParams) where T : System.Exception
		{
			(data == null).TrueThrow<T>(message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs"  lang="cs" title="ͨ���ж��������ʽboolExpression�Ľ��ֵ���ж��Ƿ��׳�ָ�����쳣��Ϣ" />
		/// <seealso cref="FalseThrow"/>
		/// <seealso cref="MCS.Library.Compression.ZipReader"/>
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.TrueThrow(name == string.Empty, "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void TrueThrow(this bool parseExpressionResult, string message, params object[] messageParams)
		{
            TrueThrow<SOASupportException>(parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ�Ĳ���</param>
		/// <typeparam name="T">�쳣������</typeparam>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ��(true)�����׳�messageָ���Ĵ�����Ϣ
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region = "TrueThrowTest" lang="cs" title="ͨ���ж��������ʽboolExpression�Ľ��ֵ���ж��Ƿ��׳�ָ�����쳣��Ϣ" />
		/// <seealso cref="FalseThrow"/>
		/// <seealso cref="MCS.Library.Logging.LogEntity"/>
		/// </remarks>
		public static void TrueThrow<T>(this bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
		{
			Type exceptionType = typeof(T);

			if (parseExpressionResult)
			{
				if (message == null)
					throw new ArgumentNullException("message");

				Object obj = Activator.CreateInstance(exceptionType);

				Type[] types = new Type[1];
				types[0] = typeof(string);

				ConstructorInfo constructorInfoObj = exceptionType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public, null,
					CallingConventions.HasThis, types, null);

				Object[] args = new Object[1];

				args[0] = string.Format(message, messageParams);

				constructorInfoObj.Invoke(obj, args);

				throw (Exception)obj;
			}
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�strMessageָ���Ĵ�����Ϣ
		/// </summary>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region = "FalseThrowTest" lang="cs" title="ͨ���ж��������ʽboolExpression�Ľ��ֵ���ж��Ƿ��׳�ָ�����쳣��Ϣ" />
		/// <seealso cref="TrueThrow"/>
		/// <seealso cref="MCS.Library.Logging.LoggerFactory"/>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�messageָ���Ĵ�����Ϣ
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.FalseThrow(name != string.Empty, "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void FalseThrow(this bool parseExpressionResult, string message, params object[] messageParams)
		{
			TrueThrow(false == parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�messageָ���Ĵ�����Ϣ
		/// </summary>
		/// <typeparam name="T">�쳣������</typeparam>
		/// <param name="parseExpressionResult">�������ʽ</param>
		/// <param name="message">������Ϣ</param>
		/// <param name="messageParams">������Ϣ����</param>
		/// <remarks>
		/// ����������ʽboolExpression�Ľ��ֵΪ�٣�false�������׳�strMessageָ���Ĵ�����Ϣ
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region="FalseThrowTest" lang="cs" title="ͨ���ж��������ʽboolExpression�Ľ��ֵ���ж��Ƿ��׳�ָ�����쳣��Ϣ" />
		/// <seealso cref="TrueThrow"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		/// <example>
		/// <code>
		/// ExceptionTools.FalseThrow(name != string.Empty, typeof(ApplicationException), "�Բ������ֲ���Ϊ�գ�");
		/// </code>
		/// </example>
		public static void FalseThrow<T>(this bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
		{
			TrueThrow<T>(false == parseExpressionResult, message, messageParams);
		}

		/// <summary>
		/// ����ַ��������Ƿ�ΪNull��մ�������ǣ����׳��쳣
		/// </summary>
		/// <param name="data">�ַ�������ֵ</param>
		/// <param name="paramName">�ַ�������</param>
		/// <remarks>
		/// ���ַ�������ΪNull��մ����׳�ArgumentException�쳣
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region="CheckStringIsNullOrEmpty" lang="cs" title="����ַ��������Ƿ�ΪNull��մ������ǣ����׳��쳣" />
		/// </remarks>
		public static void CheckStringIsNullOrEmpty(this string data, string paramName)
		{
			if (string.IsNullOrEmpty(data))
				throw new ArgumentException(string.Format("�ַ�������{0}����ΪNull��մ�", paramName));
		}

		/// <summary>
		/// ����ַ��������Ƿ�ΪNull��մ�������ǣ����׳��쳣
		/// </summary>
		/// <typeparam name="T">�쳣������</typeparam>
		/// <param name="data">����ַ��������Ƿ�ΪNull��մ�������ǣ����׳��쳣</param>
		/// <param name="message"></param>
		public static void CheckStringIsNullOrEmpty<T>(this string data, string message) where T : System.Exception
		{
			(string.IsNullOrEmpty(data)).TrueThrow(message);
		}

		/// <summary>
		/// ��Exception�����У���ȡ������������Ĵ������
		/// </summary>
		/// <param name="ex">Exception����</param>
		/// <returns>������������Ĵ������</returns>
		public static Exception GetRealException(Exception ex)
		{
			System.Exception lastestEx = ex;

			if (ex is SoapException)
			{
                lastestEx = new SOASupportException(GetSoapExceptionMessage(ex), ex);
			}
			else
			{
				while (ex != null &&
					(ex is System.Web.HttpUnhandledException || ex is System.Web.HttpException || ex is TargetInvocationException))
				{
					if (ex.InnerException != null)
						lastestEx = ex.InnerException;
					else
						lastestEx = ex;

					ex = ex.InnerException;
				}
			}

			return lastestEx;
		}

		/// <summary>
		/// �õ�SoapException�еĴ�����Ϣ
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetSoapExceptionMessage(Exception ex)
		{
			string strNewMsg = ex.Message;

			if (ex is SoapException)
			{
				int i = strNewMsg.LastIndexOf("--> ");

				if (i > 0)
				{
					strNewMsg = strNewMsg.Substring(i + 4);
					i = strNewMsg.IndexOf(": ");

					if (i > 0)
					{
						strNewMsg = strNewMsg.Substring(i + 2);

						i = strNewMsg.IndexOf("\n   ");

						strNewMsg = strNewMsg.Substring(0, i);
					}
				}
			}

			return strNewMsg;
		}
	}
}
