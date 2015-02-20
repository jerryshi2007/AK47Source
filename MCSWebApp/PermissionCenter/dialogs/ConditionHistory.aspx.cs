using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Dialogs
{
	public partial class ConditionHistory : System.Web.UI.Page
	{
		public PC.SCSimpleObject Object
		{
			get
			{
				return this.ViewState["obj"] as PC.SCSimpleObject;
			}

			set
			{
				this.ViewState["obj"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				this.Object = PC.Adapters.SchemaObjectAdapter.Instance.Load(this.Request.QueryString["id"]).ToSimpleObject();
			}

			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			this.binding1.Data = this.Object;
		}
	}
}