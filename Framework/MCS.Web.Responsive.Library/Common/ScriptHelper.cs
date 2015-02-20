using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
	public static class ScriptHelper
	{
		/// <summary>
		/// 检查脚本中的字符串，替换掉双引号和回车
		/// </summary>
		/// <param name="strData">字符串</param>
		/// <returns>替换后的结果</returns>
		public static string CheckScriptString(string strData)
		{
			return CheckScriptString(strData, true);
		}

		/// <summary>
		/// 检查脚本中的字符串，替换掉双引号和回车
		/// </summary>
		/// <param name="strData">字符串</param>
		/// <param name="changeCRToBR">是否将回车替换成Html的BR</param>
		/// <returns>替换后的结果</returns>
		public static string CheckScriptString(string strData, bool changeCRToBR)
		{
			if (strData.IsNotEmpty())
			{
				strData = strData.Replace("\\", "\\\\");
				strData = strData.Replace("\"", "\\\"");
				strData = strData.Replace("/", "\\/");
				strData = strData.Replace("\r\n", "\\n");
				strData = strData.Replace("\n\r", "\\n");
				strData = strData.Replace("\n", "\\n");
				strData = strData.Replace("\r", "\\n");

				if (changeCRToBR)
					strData = strData.Replace("\\n", "<br/>");
			}

			return strData;
		}

		/// <summary>
		/// 得到客户端弹出对话框的脚本
		/// </summary>
		/// <param name="strMessage"></param>
		/// <param name="strDetail"></param>
		/// <param name="strTitle"></param>
		/// <returns></returns>
		public static string GetShowClientErrorScript(string strMessage, string strDetail, string strTitle)
		{
			return string.Format("$HGRootNS.HGClientMsg.stop(\"{0}\", \"{1}\", \"{2}\");",
				CheckScriptString(strMessage), CheckScriptString(strDetail), CheckScriptString(strTitle));
		}
	}
}
