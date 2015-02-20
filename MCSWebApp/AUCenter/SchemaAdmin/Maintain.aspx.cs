using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using System.Transactions;
using MCS.Library.Principal;

namespace AUCenter.SchemaAdmin
{
	public partial class Maintain : System.Web.UI.Page
	{
		public override void ProcessRequest(HttpContext context)
		{
			if (AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current) == false)
				context.Response.Redirect("~/Default.aspx");
			else
			{
				if (context.Request.QueryString["action"] == "genSnapshot")
				{
					context.Server.ScriptTimeout = 60 * 60;
					AUCommon.DoDbAction(() =>
						SCSnapshotBasicAdapter.Instance.GenerateAllSchemaSnapshot());

					context.Response.ContentType = "image/gif";
					context.Response.WriteFile("~/images/ajax-loader2.gif");
				}
				else
				{
					base.ProcessRequest(context);
				}
			}
		}

		protected void GenSnapshot(object sender, EventArgs e)
		{
			AUCommon.DoDbAction(() => SCSnapshotBasicAdapter.Instance.GenerateAllSchemaSnapshot());
		}

		protected void GenSchemaTable(object sender, EventArgs e)
		{
			SchemaDefineCollection schemas = SchemaExtensions.CreateSchemasDefineFromConfiguration();

			AUCommon.DoDbAction(() =>
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					schemas.ForEach(schema => SchemaDefineAdapter.Instance.Update(schema));
					scope.Complete();
				}
			});
		}
	}
}