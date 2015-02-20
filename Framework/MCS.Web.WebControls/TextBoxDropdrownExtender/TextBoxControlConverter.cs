using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	public class TextBoxControlConverter : ControlIDConverter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		protected override bool FilterControl(Control control)
		{
			return (control is TextBox);
		}
	}
}
