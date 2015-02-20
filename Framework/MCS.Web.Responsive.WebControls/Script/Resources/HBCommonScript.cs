#region
// -------------------------------------------------
// Assembly	：	MCS.Web.Responsive.WebControls
// FileName	：	HBCommonScript.cs
// Remark	：	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			fengll		20140513		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Reflection;
using MCS.Library.Core;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Resources;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.Script.Resources.HBCommon.js", "application/x-javascript")]

namespace MCS.Web.Responsive.WebControls
{
    public enum ControlShowingMode
    {
        /// <summary>
        /// 正常模式，在页面上显示
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 模式对话框
        /// </summary>
        Dialog
    }

    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(ClientMsgResources), 2)]
    [ClientScriptResource(null, "MCS.Web.Responsive.WebControls.Script.Resources.HBCommon.js")]
    public class HBCommonScript
    {
        /// <summary>
        /// 注册AddNamespace的脚本
        /// </summary>
        public static void RegisterAddNamespaceScript()
        {
            WebUtility.GetCurrentPage().ClientScript.RegisterClientScriptBlock(
                typeof(HBCommonScript),
                "addNamespace",
                ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                    "MCS.Web.Responsive.WebControls.Script.Resources.addNamespace.js"),
                    true);
        }
    }
}
//namespace MCS.Web.Responsive.WebControls.Script { }
