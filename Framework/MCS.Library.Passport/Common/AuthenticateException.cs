#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	AuthenticateException.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
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
	/// 认证异常
	/// </summary>
	[Serializable]
	public class AuthenticateException : System.Exception
	{
		/// <summary>
		/// 认证异常
		/// </summary>
		public AuthenticateException()
			: base()
		{
		}
		/// <summary>
		/// 认证异常
		/// </summary>
		/// <param name="message">异常信息</param>
		public AuthenticateException(string message)
			: base(message)
		{
		}
		/// <summary>
		/// 认证异常
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="ex">内部异常</param>
		public AuthenticateException(string message, System.Exception ex)
			: base(message, ex)
		{
		}

		/// <summary>
		/// 抛出认证的异常
		/// </summary>
		/// <param name="logonName"></param>
		public static void ThrowAuthenticateException(string logonName)
		{
			throw new AuthenticateException(string.Format(Translator.Translate(Define.DefaultCategory, Resource.UserAuthenticateFail), logonName));
		}
	}
}
