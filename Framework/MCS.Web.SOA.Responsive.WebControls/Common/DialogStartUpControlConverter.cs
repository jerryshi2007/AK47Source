using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
{
	public class DialogStartUpControlConverter : ControlIDConverter
	{
		protected override bool FilterControl(Control control)
		{
			return (control is IAttributeAccessor);
		}
	}
}
