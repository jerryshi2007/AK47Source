using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public class ButtonControlTranslator : ControlTranslatorGenericBase<IButtonControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public ButtonControlTranslator(IButtonControl control)
			: base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			if (CategoryDefined)
				Control.Text = Translate(Control.Text);
		}
	}
}
