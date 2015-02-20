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
		/// ��ȡ��Դ�е�ָ����Դ
		/// </summary>
		/// <param name="strPath">ָ����Դ�ı�ʶ</param>
		/// <returns>��Դ�е���������������Ϣ</returns>
		public static string GetEmbeddedResString(string strPath)
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			Stream stm = currentAssembly.GetManifestResourceStream(strPath);

			StreamReader sr = new StreamReader(stm, Encoding.GetEncoding("gb2312"));

			return sr.ReadToEnd();
		}

		/// <summary>
		/// ���쳣ex���жϴ������Exception������SoapException���������ȡ�������Ĵ�����Ϣ
		/// </summary>
		/// <param name="ex">�쳣����</param>
		/// <returns>�쳣�а������������쳣��Ϣ</returns>
		/// <remarks>
		/// ���쳣ex���жϴ������Exception������SoapException���������ȡ�������Ĵ�����Ϣ��
		/// �������SoapException�������Ѹ��쳣�еĴ�����Ϣ��Message���ԣ�����
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
