using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter.Dialogs
{
	public partial class FrontDeleteDialog : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.Request.HttpMethod == "POST")
			{
				this.context.Value = HttpContext.Current.Request["context"];
				switch (this.context.Value)
				{
					case "Users":
					case "Orgnizations":
					case "Groups":
					case "Applications":
					case "Roles":
						break;
					default:
						throw new ArgumentOutOfRangeException("context");
				}

				string[] keys = HttpContext.Current.Request.Form.GetValues("keys");
				if (keys != null)
				{
					SCSimpleObjectCollection coll = new SCSimpleObjectCollection();
					MCS.Library.Data.Builder.InSqlClauseBuilder inSql = new MCS.Library.Data.Builder.InSqlClauseBuilder("ID");
					inSql.AppendItem<string>(keys);
					var dataItems = SchemaObjectAdapter.Instance.Load(inSql).FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
					SchemaToSimpleEnumerator bridge = new SchemaToSimpleEnumerator(dataItems);
					this.mainList.DataSource = bridge;
					this.mainList.DataBind();
				}
			}
		}

		protected DeleteStatus DeleteObject(string objId, string context)
		{
			if (context == "Users")
			{
				return new DeleteStatus() { Success = true, Message = "您删除了对象ID为" + objId };
			}

			throw new InvalidOperationException();
		}

		public class DeleteStatus
		{
			public bool Success { get; set; }

			public string Message { get; set; }
		}
	}
}