using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Library
{
	/// <summary>
	/// For Label, TextBox
	/// </summary>
	public class TextControlTranslator : ControlTranslatorGenericBase<ITextControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public TextControlTranslator(ITextControl control)
			: base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			if (CategoryDefined)
			{
				this.Control.Text = Translate(this.Control.Text);
			}
		}
	}
}
