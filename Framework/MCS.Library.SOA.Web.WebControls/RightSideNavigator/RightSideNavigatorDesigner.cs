using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;

namespace MCS.Web.WebControls
{
	public class RightSideNavigatorDesigner : ControlDesigner
	{
		public override string GetDesignTimeHtml()
		{
			var ctrl = Component as RightSideNavigator;
			var writer = new StringWriter();
			new HtmlTextWriter(writer);
			if (ctrl != null)
			{
				writer.WriteLine(ctrl.ID);
			}

			return writer.ToString();
		}
	}
}
