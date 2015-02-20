using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.dialogs
{
	public partial class ExpressionHint : System.Web.UI.Page
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			string schemaType = Request.QueryString["schemaType"];

			this.lblSchemaName.Text = schemaType;
			this.propRepeater.DataSource = SchemaDefine.GetSchema(schemaType).Properties;
			this.propRepeater.DataBind();
		}
	}
}