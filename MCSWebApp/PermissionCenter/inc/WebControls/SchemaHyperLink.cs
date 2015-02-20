using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace PermissionCenter.WebControls
{
	[DefaultProperty("ObjectID")]
	[ToolboxData("<{0}:SchemaHyperLink runat=server></{0}:SchemaHyperLink>")]
	public class SchemaHyperLink : HyperLink
	{
		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue("")]
		public string ObjectID
		{
			get
			{
				string s = (string)this.ViewState["ObjectID"];
				return (s == null) ? string.Empty : s;
			}

			set
			{
				this.ViewState["ObjectID"] = value;
			}
		}

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue("")]
		[Localizable(true)]
		public string ObjectSchemaType
		{
			get
			{
				string s = (string)this.ViewState["ObjectSchemaType"];
				return (s == null) ? string.Empty : s;
			}

			set
			{
				this.ViewState["ObjectSchemaType"] = value;
			}
		}

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(false)]
		[Localizable(true)]
		public bool OUViewMode
		{
			get
			{
				object s = ViewState["OUViewMode"];
				return (s == null) ? false : (bool)s;
			}

			set
			{
				this.ViewState["OUViewMode"] = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (string.IsNullOrEmpty(this.ObjectSchemaType) == false)
			{
				System.Collections.Specialized.HybridDictionary dic = Util.GetSchemaCatgoryDictionary();

				if (dic.Contains(this.ObjectSchemaType))
				{
					switch ((string)dic[this.ObjectSchemaType])
					{
						case "Roles":
						case "Applications":
						case "Groups":
							this.Attributes["onclick"] = "return $pc.modalPopup(this);";
							break;
						default:
							break;
					}

					if (string.IsNullOrEmpty(this.ObjectID) == false)
					{
						switch ((string)dic[this.ObjectSchemaType])
						{
							case "Organizations":
								this.NavigateUrl = this.OUViewMode ? ("~/lists/OUExplorerView.aspx?ou=" + HttpUtility.UrlEncode(this.ObjectID)) : ("~/lists/OUExplorer.aspx?ou=" + HttpUtility.UrlEncode(this.ObjectID));
								break;
							case "Roles":
								this.NavigateUrl = "~/lists/AppRoleMembers.aspx?role=" + HttpUtility.UrlEncode(this.ObjectID);
								break;
							case "Applications":
								this.NavigateUrl = "~/lists/AppRoles.aspx?app=" + HttpUtility.UrlEncode(this.ObjectID);
								break;
							case "Groups":
								this.NavigateUrl = "~/lists/GroupConstMembers.aspx?id=" + HttpUtility.UrlEncode(this.ObjectID);
								break;
						}
					}
				}
			}
		}
	}
}
