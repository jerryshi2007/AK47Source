using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.RoleControl.RoleGraphControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.RoleGraphControl", "MCS.Web.WebControls.RoleControl.RoleGraphControl.js")]
	[DialogContent("MCS.Web.WebControls.RoleControl.roleGraphDialogTemplate.htm")]
	[ToolboxData("<{0}:RoleGraphControl runat=server></{0}:RoleGraphControl>")]
	public class RoleGraphControl : DialogControlBase<RoleGraphControlParams>
	{
		private HtmlSelect _ApplicationSelector = null;
		private HtmlSelect _RoleSelector = null;
		private HtmlGenericControl _LoadingTag = null;
		private HtmlAnchor _RelativeLink = null;

		private OguApplicationCollection _ApplicationsData = new OguApplicationCollection();
		private OguRoleCollection _RolesData = new OguRoleCollection();

		public RoleGraphControl()
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
			JSONSerializerExecute.RegisterConverter(typeof(OguApplicationConverter));
		}

		[ScriptControlProperty]
		[ClientPropertyName("selectedFullCodeName")]
		public string SelectedFullCodeName
		{
			get
			{
				return ControlParams.SelectedFullCodeName;
			}
			set
			{
				ControlParams.SelectedFullCodeName = value;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("applicationSelectorClientID")]
		private string ApplicationSelectorClientID
		{
			get
			{
				return this._ApplicationSelector != null ? this._ApplicationSelector.ClientID : string.Empty;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("roleSelectorClientID")]
		private string RoleSelectorClientID
		{
			get
			{
				return this._RoleSelector != null ? this._RoleSelector.ClientID : string.Empty;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("loadingTagClientID")]
		private string LoadingTagClientID
		{
			get
			{
				return this._LoadingTag != null ? this._LoadingTag.ClientID : string.Empty;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("relativeLinkClientID")]
		private string RelativeLinkClientID
		{
			get
			{
				return this._RelativeLink != null ? this._RelativeLink.ClientID : string.Empty;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("applicationsData")]
		private OguApplicationCollection ApplicationsData
		{
			get
			{
				return this._ApplicationsData;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("rolesData")]
		private OguRoleCollection RolesData
		{
			get
			{
				return this._RolesData;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("relativeLinkTemplate")]
		private string RelativeLinkTemplate
		{
			get
			{
				string result = "/MCSWebApp/PermissionCenter/lists/AppRoles.aspx?appCodeName={0}";

				UriConfigurationCollection urls = ResourceUriSettings.GetConfig().Paths;

				if (urls.ContainsKey("appRoles"))
					result = urls["appRoles"].Uri.ToString();

				return result;
			}
		}

		[ScriptControlMethod]
		public OguRoleCollection GetAppRoles(string codeName)
		{
			ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications(codeName);

			OguRoleCollection roles = new OguRoleCollection();

			if (apps.Count > 0)
				roles.CopyFrom(apps[0].Roles);

			return roles;
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			if (this.DialogTitle.IsNullOrEmpty())
				this.DialogTitle = RoleGraphControlParams.DefaultDialogTitle;

			this._ApplicationSelector = (HtmlSelect)container.FindControlByID("applications", true);
			this._RoleSelector = (HtmlSelect)container.FindControlByID("roles", true);
			this._LoadingTag = (HtmlGenericControl)container.FindControlByID("loadingTag", true);
			this._RelativeLink = (HtmlAnchor)container.FindControlByID("relativeLink", true);

			HtmlGenericControl img = (HtmlGenericControl)container.FindControlByID("loadingImg", true);

			if (img != null)
				img.Style["background-image"] = string.Format("url({0})", ControlResources.HourglassLogoUrl);

			OguApplicationCollection apps = new OguApplicationCollection(PermissionMechanismFactory.GetMechanism().GetAllApplications());

			this._ApplicationsData = apps;

			if (apps.Count > 0)
			{
				string appCodeName = GetAppCodeNameFromFullCodeName(this.SelectedFullCodeName);

				IApplication selectedApp = null;

				if (appCodeName.IsNotEmpty())
					selectedApp = apps.Find(app => app.CodeName == appCodeName);

				if (selectedApp == null)
					selectedApp = apps[0];

				this._RolesData = new OguRoleCollection(selectedApp.Roles);
			}
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Attributes["onclick"] = string.Format("window.returnValue = $find(\"{0}\")._collectResult();window.close();", this.ClientID);
		}

		protected override void InitMiddleButton(HtmlInputButton middleButton)
		{
			middleButton.Value = "清空";
			middleButton.Style["display"] = "inline";
			middleButton.Attributes["onclick"] = string.Format("$find(\"{0}\").set_selectedFullCodeName(\"\");window.returnValue = \"\"; window.close();", this.ClientID);
		}

		protected override void LoadClientState(string clientState)
		{
			base.LoadClientState(clientState);

			if (clientState.IsNotEmpty())
				this.SelectedFullCodeName = clientState;
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 420;
			feature.Height = 320;
			feature.Center = true;
			feature.Resizable = false;
			feature.ShowScrollBars = false;
			feature.ShowStatusBar = false;

			return feature.ToDialogFeatureClientString();
		}

		private static OguApplicationCollection InitApplicationsData(string fullCodeName)
		{
			OguApplicationCollection result = null;

			string appCodeName = GetAppCodeNameFromFullCodeName(fullCodeName);

			if (appCodeName.IsNullOrEmpty())
			{
				result = new OguApplicationCollection(PermissionMechanismFactory.GetMechanism().GetAllApplications());
			}
			else
			{
				result = new OguApplicationCollection(PermissionMechanismFactory.GetMechanism().GetApplications(appCodeName));

				if (result.Count == 0)
					result = new OguApplicationCollection(PermissionMechanismFactory.GetMechanism().GetAllApplications());
			}

			return result;
		}

		private static string GetAppCodeNameFromFullCodeName(string fullCodeName)
		{
			string result = string.Empty;

			if (fullCodeName.IsNotEmpty())
				result = fullCodeName.Split(':')[0];

			return result;
		}
	}
}
