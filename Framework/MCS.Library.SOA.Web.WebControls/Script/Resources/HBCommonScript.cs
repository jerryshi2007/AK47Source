#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	HBCommonScript.cs
// Remark	��	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070731		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Resources;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using System.Reflection;
using MCS.Library.Core;

[assembly: WebResource("MCS.Web.WebControls.Script.Resources.HBCommon.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	public enum ControlShowingMode
	{
		/// <summary>
		/// ����ģʽ����ҳ������ʾ
		/// </summary>
		Normal = 0,

		/// <summary>
		/// ģʽ�Ի���
		/// </summary>
		Dialog
	}

	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(ClientMsgResources), 2)]
	[ClientScriptResource(null, "MCS.Web.WebControls.Script.Resources.HBCommon.js")]
	public class HBCommonScript
	{
		/// <summary>
		/// ע��AddNamespace�Ľű�
		/// </summary>
		public static void RegisterAddNamespaceScript()
		{
			WebUtility.GetCurrentPage().ClientScript.RegisterClientScriptBlock(
				typeof(HBCommonScript), 
				"addNamespace",
				ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
					"MCS.Web.WebControls.Script.Resources.addNamespace.js"),
					true);
		}
	}
}
//namespace MCS.Web.WebControls.Script { }
