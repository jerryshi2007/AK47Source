using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageControlTranslator : ControlTranslatorGenericBase<Image>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public ImageControlTranslator(Image control)
			: base(control)
		{
		}

		/// <summary>
		///
		/// </summary>
		public override void Translate()
		{
			this.Control.AlternateText = Translate(this.Control.AlternateText);
		}
	}
}
