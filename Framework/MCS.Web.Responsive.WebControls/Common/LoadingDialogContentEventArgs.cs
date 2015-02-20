using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// 加载对话框内容的委托
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="eventArgs"></param>
	public delegate void LoadingDialogContentEventHander(object sender, LoadingDialogContentEventArgs eventArgs);

	/// <summary>
	/// 加载对话框内容的事件参数
	/// </summary>
	public class LoadingDialogContentEventArgs : System.EventArgs
	{
		/// <summary>
		/// 对话框的内容。如果没有指定。则使用默认的模板
		/// </summary>
		public string Content
		{
			get;
			set;
		}
	}
}
