using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using MCS.Library.Globalization;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;

namespace MCS.Web.Library
{
	internal class TranslatorControlHelper
	{
		public static void RecursiveTranslate(Control container)
		{
			TranslateControlText(container);

			foreach (Control control in container.Controls)
			{
				RecursiveTranslate(control);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static IEnumerable<IControlTranslator> GetControlTranslators(Control control)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(control != null, "control");

			List<IControlTranslator> translators = new List<IControlTranslator>();

			if (CategoryDefined(control))
			{
				if (control is WebControl)
					translators.Add(new WebControlTranslator((WebControl)control));

				if (control is ITextControl)
					translators.Add(new TextControlTranslator((ITextControl)control));

				if (control is IButtonControl)
					translators.Add(new ButtonControlTranslator((IButtonControl)control));

				if (control is HtmlControl)
					translators.Add(new HtmlControlTranslator((HtmlControl)control));

				if (control is HtmlInputControl)
					translators.Add(new HtmlInputControlTranslator((HtmlInputControl)control));

				if (control is HtmlContainerControl)
					translators.Add(new HtmlContainerControlTranslator((HtmlContainerControl)control));

				if (control is HtmlImage)
					translators.Add(new HtmlImageControlTranslator((HtmlImage)control));

				if (control is HtmlTitle)
					translators.Add(new HtmlTitleControlTranslator((HtmlTitle)control));

				if (control is IAttributeAccessor)
					translators.Add(new AttributeAccessorControlTranslator((IAttributeAccessor)control));

				if (control is HtmlSelect)
					translators.Add(new HtmlSelectControlTranslator((HtmlSelect)control));

				if (control is ListControl)
					translators.Add(new ListControlTranslator((ListControl)control));
			}

			return translators;
		}

		private static void TranslateControlText(Control control)
		{
			IEnumerable<IControlTranslator> translators = GetControlTranslators(control);

			foreach (IControlTranslator translator in translators)
				translator.Translate();
		}

		private static bool CategoryDefined(Control control)
		{
			bool result = false;

			if (control is IAttributeAccessor)
			{
				IAttributeAccessor accessor = (IAttributeAccessor)control;

				result = string.IsNullOrEmpty(accessor.GetAttribute("Category")) == false;
			}

			return result;
		}
	}
}
