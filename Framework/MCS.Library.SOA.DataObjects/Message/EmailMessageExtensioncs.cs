using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public static class EmailMessageExtensioncs
	{
		/// <summary>
		/// 转换为.Net的MailAddress
		/// </summary>
		/// <param name="ea"></param>
		/// <returns></returns>
		public static MailAddress ToMailAddress(this EmailAddress ea)
		{
			MailAddress result = null;

			if (ea != null)
				result = new MailAddress(ea.Address, ea.DisplayName);

			return result;
		}

		/// <summary>
		/// 将Encoding转换成名称，如果为空，则返回空串
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ToDescription(this Encoding encoding)
		{
			string result = string.Empty;

			if (encoding != null)
				result = encoding.BodyName;

			return result;
		}

		/// <summary>
		/// 将名称转换成Encoding
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="encodingDesp"></param>
		/// <returns></returns>
		public static Encoding FromDescription(this Encoding encoding, string encodingDesp)
		{
			Encoding result = Encoding.UTF8;

			if (encodingDesp.IsNotEmpty())
				result = Encoding.GetEncoding(encodingDesp);

			return result;
		}
	}
}
