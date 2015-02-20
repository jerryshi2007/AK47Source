using System;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;

namespace MCS.Web.Library
{
	/// <summary>
	/// 支持本地化的User Control的基类
	/// </summary>
	public abstract class GlobalizationUserControlBase : UserControl
	{
		/// <summary>
		/// Init
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			base.OnInit(e);
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			TranslatorControlHelper.RecursiveTranslate(this);
		}
	}
}
