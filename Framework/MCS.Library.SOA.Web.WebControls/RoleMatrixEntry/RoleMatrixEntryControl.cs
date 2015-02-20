using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Globalization;

[assembly: WebResource("MCS.Web.WebControls.Images.excel.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.property.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	[ToolboxData("<{0}:RoleMatrixEntryControl runat=server />")]
	public class RoleMatrixEntryControl : Control
	{
		private struct RoleIDContainer
		{
			public string AppID;
			public string RoleID;
		}

		#region Properties
		// <summary>
		/// Application ID
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Application ID")]
		public string AppID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AppID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AppID", value);
			}
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Application CodeName")]
		public string AppCodeName
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AppCodeName", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AppCodeName", value);
			}
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Application Name")]
		public string AppName
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AppName", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AppName", value);
			}
		}

		// <summary>
		/// Role ID
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Role ID")]
		public string RoleID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "RoleID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "RoleID", value);
			}
		}

		// <summary>
		/// RoleName
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Role Name")]
		public string RoleName
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "RoleName", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "RoleName", value);
			}
		}

		// <summary>
		/// Role Code Name
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Role Code Name")]
		public string RoleCodeName
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "RoleCodeName", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "RoleCodeName", value);
			}
		}

		// <summary>
		/// Role Code Name
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Role Description")]
		public string RoleDescription
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "RoleDescription", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "RoleDescription", value);
			}
		}

		// <summary>
		/// 属性定义的ID，如果为空，则使用RoleID作为属性定义的ID
		/// </summary>
		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		[Description("Definition ID")]
		public string DefinitionID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "DefinitionID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "DefinitionID", value);
			}
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(true)]
		[Localizable(true)]
		[Description("Enabled")]
		public bool Enabled
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "Enabled", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Enabled", value);
			}
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue(true)]
		[Localizable(true)]
		[Description("是否启用只读模式")]
		public bool ReadOnly
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ReadOnly", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ReadOnly", value);
			}
		}

		[Bindable(true)]
		[Category("Security")]
		[DefaultValue(false)]
		[Localizable(true)]
		[Description("是否启用票据检查")]
		public bool EnableAccessTicket
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "EnableAccessTicket", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "EnableAccessTicket", value);
			}
		}

		#endregion Properties

		#region Protected
		protected override void OnLoad(EventArgs e)
		{
			Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
				"RoleMatrixEntryControlScript",
				ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.RoleMatrixEntry.RoleMatrixEntryControlScript.htm"),
				false);

			AccessTicketHtmlAnchor.RegisterScript(this.Page);

			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
				writer.Write("RoleMatrixEntryControl");
			else
				RenderRoleMatrixEntry(writer);
		}

		#endregion Protected

		#region Private
		private string EditMatrixImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(RoleMatrixEntryControl),
						"MCS.Web.WebControls.Images.excel.gif");
			}
		}

		private string EditPropertiesImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(RoleMatrixEntryControl),
						"MCS.Web.WebControls.Images.property.gif");
			}
		}

		private void RenderRoleMatrixEntry(HtmlTextWriter writer)
		{
			if (this.RoleID.IsNotEmpty())
			{
				HtmlGenericControl container = new HtmlGenericControl("div");

				container.Controls.Add(CreateLinkButton(GetEditPropertiesUrl(), "_ppt",
					EditPropertiesImgUrl,
					Translator.Translate(Define.DefaultCulture, this.Enabled ? "编辑矩阵属性" : string.Empty),
					GetEditPropertiesLinkScript()));

				bool exsits = false;

				HaveExtendedPropertiesRoleIDs.TryGetValue(this.RoleID, out exsits);

				if (exsits)
				{
					container.Controls.Add(CreateLinkButton(GetEditMatrixUrl(), "_mtx",
						EditMatrixImgUrl,
						Translator.Translate(Define.DefaultCulture, this.Enabled ? "编辑矩阵属性" : "存在矩阵属性定义"),
						GetEditMatrixLinkScript()));
				}

				writer.Write(WebControlUtility.GetControlHtml(container));
			}
		}

		private string GetEditPropertiesLinkScript()
		{
			string result = "return onRoleMatrixEditPropertiesClick(this)";

			if (this.EnableAccessTicket)
				result = "onRoleMatrixEditPropertiesClick";

			return result;
		}

		private string GetEditMatrixLinkScript()
		{
			string result = "return onRoleMatrixEditMatrixClick(this)";

			if (this.EnableAccessTicket)
				result = "onRoleMatrixEditMatrixClick";

			return result;
		}

		private Control CreateLinkButton(string linkUrl, string prefix, string imgUrl, string title, string linkScript)
		{
			HtmlAnchor anchor = null;

			if (this.EnableAccessTicket)
			{
				anchor = new AccessTicketHtmlAnchor();

				anchor.ID = this.ClientID + prefix + "_anchor";
				if (linkScript.IsNotEmpty())
					((AccessTicketHtmlAnchor)anchor).OnClientAccquiredAccessTicket = linkScript;
			}
			else
			{
				anchor = new HtmlAnchor();

				if (linkScript.IsNotEmpty())
					anchor.Attributes["onclick"] = linkScript;
			}

			anchor.HRef = linkUrl;
			anchor.Attributes["class"] = "roleMatrixLink";
			anchor.Attributes["appCodeName"] = this.AppCodeName;
			anchor.Attributes["appName"] = this.AppName;
			anchor.Attributes["roleID"] = this.RoleID;
			anchor.Attributes["appID"] = this.AppID;
			anchor.Attributes["roleName"] = this.RoleName;
			anchor.Attributes["roleCodeName"] = this.RoleCodeName;
			anchor.Attributes["roleDescription"] = this.RoleDescription;
			anchor.Attributes["definitionID"] = this.DefinitionID.IsNotEmpty() ? this.DefinitionID : this.RoleID;
			anchor.Attributes["readOnlyMode"] = this.ReadOnly ? "enabled" : "disabled";

			HtmlImage img = new HtmlImage();

			img.Src = imgUrl;
			img.Alt = title;
			img.Attributes["class"] = "roleMatrixIcon";
			anchor.Controls.Add(img);

			return anchor;
		}

		private string GetEditPropertiesUrl()
		{
			string format = "/MCSWebApp/WorkflowDesigner/MatrixModalDialog/RolePropertyExtension.aspx?AppID={0}&RoleID={1}&AppCodeName={2}&RoleCodeName={3}&editMode={4}&definitionID={5}";
			return string.Format(format,
				HttpUtility.UrlEncode(this.AppID),
				HttpUtility.UrlEncode(this.RoleID),
				HttpUtility.UrlEncode(this.AppCodeName),
				HttpUtility.UrlEncode(this.RoleCodeName),
				this.ReadOnly ? "readOnly" : "normal",
				HttpUtility.UrlEncode(this.DefinitionID)
				);
		}

		private string GetEditMatrixUrl()
		{
			return string.Format("/MCSWebApp/WorkflowDesigner/MatrixModalDialog/EditRoleProperty.aspx?AppID={0}&RoleID={1}&editMode={2}&definitionID={3}",
				HttpUtility.UrlEncode(this.AppID),
				HttpUtility.UrlEncode(this.RoleID),
				this.ReadOnly ? "readOnly" : "normal",
				HttpUtility.UrlEncode(this.DefinitionID)
				);
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			EnsureInRoleIDs();
		}

		private void EnsureInRoleIDs()
		{
			if (this.RoleID.IsNotEmpty())
			{
				RoleIDContainer container = new RoleIDContainer() { AppID = this.AppID, RoleID = this.RoleID };

				if (RoleIDs.ContainsKey(container) == false)
					RoleIDs.Add(container, container);
			}
		}

		private static Dictionary<RoleIDContainer, RoleIDContainer> RoleIDs
		{
			get
			{
				Dictionary<RoleIDContainer, RoleIDContainer> result = (Dictionary<RoleIDContainer, RoleIDContainer>)HttpContext.Current.Items["RoleMatrixEntryControl_RoleIDs"];

				if (result == null)
				{
					result = new Dictionary<RoleIDContainer, RoleIDContainer>();
					HttpContext.Current.Items["RoleMatrixEntryControl_RoleIDs"] = result;
				}

				return result;
			}
		}

		/// <summary>
		/// 得到包含扩展属性（矩阵）的角色ID字典
		/// </summary>
		private static Dictionary<string, bool> HaveExtendedPropertiesRoleIDs
		{
			get
			{
				Dictionary<string, bool> result = (Dictionary<string, bool>)HttpContext.Current.Items["RoleMatrixEntryControl_HaveExtendedPropertiesRoleIDs"];

				if (result == null)
				{
					List<string> roleIDs = new List<string>();

					foreach (KeyValuePair<RoleIDContainer, RoleIDContainer> kp in RoleIDs)
						roleIDs.Add(kp.Key.RoleID);

					result = SOARolePropertiesAdapter.Instance.AreExist(roleIDs);

					HttpContext.Current.Items["RoleMatrixEntryControl_HaveExtendedPropertiesRoleIDs"] = result;
				}

				return result;
			}
		}

		#endregion Private
	}
}
