using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test
{
	public partial class UploadFileToMossTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void submit_OnClick(object sender, EventArgs e)
		{
			MaterialAdapter.Instance.SaveDeltaMaterials(this.MaterialControl1.DeltaMaterials);
			MaterialAdapter.Instance.SaveDeltaMaterials(this.MaterialControl2.DeltaMaterials);
			MaterialAdapter.Instance.SaveDeltaMaterials(this.MaterialControl3.DeltaMaterials);
		}
	}
}