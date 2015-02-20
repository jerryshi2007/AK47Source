#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	ClientGridScript.cs
// Remark	：	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			周维海		20071217		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Resources;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.ClientGrid.ClientGrid.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 0)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource(null, "MCS.Web.WebControls.ClientGrid.ClientGrid.js")]
	public class ClientGridScript
	{

	}
}
