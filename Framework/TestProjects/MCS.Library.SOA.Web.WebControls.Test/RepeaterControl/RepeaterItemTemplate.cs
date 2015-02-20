using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace MCS.Library.SOA.Web.WebControls.Test.RepeaterControl
{
	public class RepeaterItemTemplate : ITemplate
	{
		public void InstantiateIn(Control container)
		{
			RepeaterItem item = (RepeaterItem)container;

			HtmlTableCell cell = new HtmlTableCell();
			container.Controls.Add(cell);

			cell.DataBinding += new EventHandler(cell_DataBinding);
		}

		private void cell_DataBinding(object sender, EventArgs e)
		{
			HtmlTableCell cell = (HtmlTableCell)sender;

			RepeaterItem item = (RepeaterItem)cell.NamingContainer;

			cell.InnerText= (string)DataBinder.Eval(item.DataItem, "Name");
		}
	}
}