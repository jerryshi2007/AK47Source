using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 票据检查的异常类
	/// </summary>
	public class AccessTicketCheckException : System.Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public AccessTicketCheckException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public AccessTicketCheckException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public AccessTicketCheckException(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
