using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.Services
{
	/// <summary>
	/// SoapMessage的扩展方法
	/// </summary>
	public static class SoapMessageExtensions
	{
		/// <summary>
		/// 从SoapMessage得到方法名称，去掉前面的名字空间
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string GetMethodName(this SoapMessage message)
		{
			string methodName = string.Empty;

			if (message != null)
			{
				methodName = message.Action;

				int slashIndex = methodName.LastIndexOf('/');

				if (slashIndex == -1)
					slashIndex = methodName.LastIndexOf('\\');

				if (slashIndex >= 0)
					methodName = methodName.Substring(slashIndex + 1);
			}

			return methodName;
		}
	}
}
