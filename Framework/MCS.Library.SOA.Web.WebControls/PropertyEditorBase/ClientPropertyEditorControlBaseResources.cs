using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.PropertyEditorBase.PropertyEditorControlBase.js", "text/javascript", PerformSubstitution = true)]

namespace MCS.Web.WebControls
{
	[ClientScriptResource(null, "MCS.Web.WebControls.PropertyEditorBase.PropertyEditorControlBase.js", Cacheability = ClientScriptCacheability.None)]
	public sealed class ClientPropertyEditorControlBaseResources
	{
		///// <summary>
		///// 脚本文件名
		///// </summary>
		//public static readonly string ScriptFileName = "PropertyEditorControlBase.js";

		///// <summary>
		///// 获取资源文件url
		///// </summary>
		///// <param name="clientScript"></param>
		///// <param name="fileName"></param>
		///// <returns></returns>
		//public static string GetClientMsgResourceUrl(ClientScriptManager clientScript, string fileName)
		//{
		//    return clientScript.GetWebResourceUrl(typeof(ClientPropertyEditorControlBaseResources), GetFileFullName(fileName));
		//}

		///// <summary>
		///// 获取资源文件全名
		///// </summary>
		///// <param name="fileName">资源文件</param>
		///// <returns>资源文件全名</returns>
		//public static string GetFileFullName(string fileName)
		//{
		//    return string.Format("MCS.Web.WebControls.PropertyEditorBase.{0}", fileName);
		//}
	}
}
