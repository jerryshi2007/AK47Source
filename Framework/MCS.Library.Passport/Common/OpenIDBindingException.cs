using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 绑定OpenID时的异常
	/// </summary>
	[Serializable]
	public class OpenIDBindingException : System.Exception
	{
		/// <summary>
		/// 认证异常
		/// </summary>
		public OpenIDBindingException()
			: base()
		{
		}
		/// <summary>
		/// 认证异常
		/// </summary>
		/// <param name="message">异常信息</param>
		public OpenIDBindingException(string message)
			: base(message)
		{
		}
		/// <summary>
		/// 认证异常
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="ex">内部异常</param>
		public OpenIDBindingException(string message, System.Exception ex)
			: base(message, ex)
		{
		}
	}
}
