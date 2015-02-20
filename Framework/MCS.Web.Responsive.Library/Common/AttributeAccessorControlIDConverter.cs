using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
	public class AttributeAccessorControlIDConverter : ControlIDConverter
	{
		protected override bool FilterControl(Control control)
		{
			return (control is IAttributeAccessor);
		}
	}
}
