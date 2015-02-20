using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web.Services.Protocols;

namespace MCS.Library.Accredit.WebBase
{
	class CommonDefine
	{
		/// <summary>
		/// 获取资源中的指定资源
		/// </summary>
		/// <param name="strPath">指定资源的标识</param>
		/// <returns>资源中的所有文字数据信息</returns>
		public static string GetEmbeddedResString(string strPath)
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			Stream stm = currentAssembly.GetManifestResourceStream(strPath);

			StreamReader sr = new StreamReader(stm, Encoding.GetEncoding("gb2312"));

			return sr.ReadToEnd();
		}

		/// <summary>
		/// 对异常ex的判断处理。如果Exception对象是SoapException，则从中提取中真正的错误信息
		/// </summary>
		/// <param name="ex">异常对象</param>
		/// <returns>异常中包含的真正的异常信息</returns>
		/// <remarks>
		/// 对异常ex的判断处理。如果Exception对象是SoapException，则从中提取中真正的错误信息。
		/// 如果不是SoapException，仅仅把该异常中的错误信息（Message属性）返回
		/// </remarks>
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
