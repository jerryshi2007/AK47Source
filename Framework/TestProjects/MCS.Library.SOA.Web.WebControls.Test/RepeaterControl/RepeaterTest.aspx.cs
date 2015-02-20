using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace MCS.Library.SOA.Web.WebControls.Test.RepeaterControl
{
	public partial class RepeaterTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			CreateRepeater(container);
			base.OnPreRender(e);
		}

		private static void CreateRepeater(Control parent)
		{
			Control cellContainer = PrepareCellContainer(parent);

			Repeater repeater = new Repeater();
			cellContainer.Controls.Add(repeater);

			repeater.HeaderTemplate = new RepeaterHeaderTemplate();
			repeater.ItemTemplate = new RepeaterItemTemplate();

			repeater.DataSource = TestData.PrepareData();

			repeater.DataBind();
		}

		private static Control PrepareCellContainer(Control parent)
		{
			HtmlGenericControl table = new HtmlGenericControl("table");

			HtmlGenericControl row = new HtmlGenericControl("tr");

			row.ID = "cellContainer";
			table.Controls.Add(row);

			parent.Controls.Add(table);

			return row;
		}
	}
}