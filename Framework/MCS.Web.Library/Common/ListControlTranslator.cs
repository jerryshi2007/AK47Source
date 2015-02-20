using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public class ListControlTranslator : ControlTranslatorGenericBase<ListControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public ListControlTranslator(ListControl control) :
			base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			if (CategoryDefined)
			{
				foreach (ListItem item in this.Control.Items)
				{
					item.Text = Translate(item.Text);
				}
			}
		}
	}
}
