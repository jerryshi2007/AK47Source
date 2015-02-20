using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WorkflowDesigner.ExternalDialogs
{
	/// <summary>
	/// 提供给外部的属性编辑器的基类
	/// </summary>
	public class ExternalPropertyEditorBase : Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
	}
}