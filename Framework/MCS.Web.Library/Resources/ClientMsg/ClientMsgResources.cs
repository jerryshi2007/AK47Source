using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.WebMsgBox.htm", "text/html")]
[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.ClientMsg.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.alert.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.confirm.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.inform.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.ClientMsg.stop.gif", "image/gif")]

namespace MCS.Web.Library.Resources
{
	/// <summary>
	/// 与资源脚本MCS.Web.Library.Resources.ClientMsg.ClientMsg.js"相关联的类
	/// </summary>
	[Script.RequiredScript(typeof(DeluxeScript))]
	[Script.ClientScriptResource(null, "MCS.Web.Library.Resources.ClientMsg.ClientMsg.js", Cacheability = ClientScriptCacheability.None)]
	public sealed class ClientMsgResources
	{
		/// <summary>
		/// 脚本文件名
		/// </summary>
		public static readonly string ScriptFileName = "ClientMsg.js";

		/// <summary>
		/// 获取资源文件url
		/// </summary>
		/// <param name="clientScript"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetClientMsgResourceUrl(ClientScriptManager clientScript, string fileName)
		{
			return clientScript.GetWebResourceUrl(typeof(ClientMsgResources), GetFileFullName(fileName));
		}

		/// <summary>
		/// 获取资源文件全名
		/// </summary>
		/// <param name="fileName">资源文件</param>
		/// <returns>资源文件全名</returns>
		public static string GetFileFullName(string fileName)
		{
			return string.Format("MCS.Web.Library.Resources.ClientMsg.{0}", fileName);
		}
	}
}
