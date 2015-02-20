#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	AuthenticateException.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport.Properties;
using MCS.Library.Globalization;

namespace MCS.Library.Passport
{
	/// <summary>
	/// ��֤�쳣
	/// </summary>
	[Serializable]
	public class AuthenticateException : System.Exception
	{
		/// <summary>
		/// ��֤�쳣
		/// </summary>
		public AuthenticateException()
			: base()
		{
		}
		/// <summary>
		/// ��֤�쳣
		/// </summary>
		/// <param name="message">�쳣��Ϣ</param>
		public AuthenticateException(string message)
			: base(message)
		{
		}
		/// <summary>
		/// ��֤�쳣
		/// </summary>
		/// <param name="message">�쳣��Ϣ</param>
		/// <param name="ex">�ڲ��쳣</param>
		public AuthenticateException(string message, System.Exception ex)
			: base(message, ex)
		{
		}

		/// <summary>
		/// �׳���֤���쳣
		/// </summary>
		/// <param name="logonName"></param>
		public static void ThrowAuthenticateException(string logonName)
		{
			throw new AuthenticateException(string.Format(Translator.Translate(Define.DefaultCategory, Resource.UserAuthenticateFail), logonName));
		}
	}
}
