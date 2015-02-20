using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.RepeaterControl
{
	public class RepeaterHeaderTemplate : ITemplate
	{
		public void InstantiateIn(Control container)
		{
			RepeaterItem item = (RepeaterItem)container;

			HtmlTableCell cell = new HtmlTableCell();

			cell.InnerText = "<All>";

			container.Controls.Add(cell);
		}
	}
}