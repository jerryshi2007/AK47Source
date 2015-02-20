using System;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace MCS.Web.Responsive.Library
{
	/// <summary>
	/// 
	/// </summary>
	public class HtmlControlTranslator : ControlTranslatorGenericBase<HtmlControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public HtmlControlTranslator(HtmlControl control)
			: base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HtmlInputControlTranslator : ControlTranslatorGenericBase<HtmlInputControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public HtmlInputControlTranslator(HtmlInputControl control)
			: base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			Control.Value = Translate(Control.Value);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HtmlContainerControlTranslator : ControlTranslatorGenericBase<HtmlContainerControl>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public HtmlContainerControlTranslator(HtmlContainerControl control) :
			base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			this.Control.InnerText = Translate(this.Control.InnerText);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HtmlTitleControlTranslator : ControlTranslatorGenericBase<HtmlTitle>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public HtmlTitleControlTranslator(HtmlTitle control) :
			base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			this.Control.Text = Translate(this.Control.Text);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HtmlImageControlTranslator : ControlTranslatorGenericBase<HtmlImage>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public HtmlImageControlTranslator(HtmlImage control) :
			base(control)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Translate()
		{
			Control.Alt = Translate(this.Control.Alt);
		}
	}
}
