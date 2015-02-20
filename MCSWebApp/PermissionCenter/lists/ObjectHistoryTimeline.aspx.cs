using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PermissionCenter
{
	public partial class ObjectHistoryTimeline : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var id = Request.QueryString["id"];
			var encodeId = Server.UrlEncode(id);
			this.lnkList.NavigateUrl = "~/lists/ObjectHistoryLog.aspx?id=" + encodeId;
			this.lnkTimeline.NavigateUrl = "~/lists/ObjectHistoryTimeline.aspx?id=" + encodeId;
			var obj = MCS.Library.SOA.DataObjects.Security.Adapters.SchemaObjectAdapter.Instance.Load(id);
			if (obj != null)
			{
				this.schemaTypeLabel.InnerText = obj.SchemaType;
				this.objId.Value = obj.ID;
			}
		}
	}
}