using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Responsive.Library.Script;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Library.Configuration;
using MCS.Library.OGUPermission;

[assembly: WebResource("MCS.Web.Responsive.WebControls.RoleGraphControl.RoleGraphControl.js", "text/javascript")]

namespace MCS.Web.Responsive.WebControls
{
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.RoleGraphControl", "MCS.Web.Responsive.WebControls.RoleGraphControl.RoleGraphControl.js")]
    [DialogContent("MCS.Web.Responsive.WebControls.RoleGraphControl.roleGraphDialogTemplate.htm", "MCS.Web.SOA.Responsive.WebControls")]
    [ToolboxData("<{0}:RoleGraphControl runat=server></{0}:RoleGraphControl>")]
    [PropertyEditorDescription("RoleGraphControl", "MCS.Web.WebControls.RoleGraphControl")]
    public class RoleGraphControl : DialogControlBase, INamingContainer  // <RoleGraphControlParams>
    {
        private HtmlSelect _ApplicationSelector = null;
        private HtmlSelect _RoleSelector = null;
        private HtmlGenericControl _LoadingTag = null;
        private HtmlAnchor _RelativeLink = null;
        private HtmlInputHidden hiddenFullCodeName;

        private OguApplicationCollection _ApplicationsData = new OguApplicationCollection();
        private OguRoleCollection _RolesData = new OguRoleCollection();

        public RoleGraphControl()
        {
            this.hiddenFullCodeName = new HtmlInputHidden() { ID = "hiddenFullCodeName" };
            JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
            JSONSerializerExecute.RegisterConverter(typeof(OguApplicationConverter));
        }

        [ScriptControlProperty]
        [ClientPropertyName("selectedFullCodeName")]
        public string SelectedFullCodeName
        {
            get
            {
                return this.hiddenFullCodeName.Value;  //return ControlParams.SelectedFullCodeName;
            }
            set
            {
                this.hiddenFullCodeName.Value = value; // ControlParams.SelectedFullCodeName = value;
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Controls.Add(this.hiddenFullCodeName);
            if (CurrentMode == ControlShowingMode.Dialog)
                this.SelectedFullCodeName = this.Page.Request.QueryString["selectedFullCodeName"];
        }


        protected override void OnDialogContentControlLoaded(Control container)
        {
            base.OnDialogContentControlLoaded(container);

            if (string.IsNullOrEmpty(this.DialogTitle))
                this.DialogTitle = RoleGraphControlParams.DefaultDialogTitle;

            this._ApplicationSelector = (HtmlSelect)container.FindControl("applications");
            this._RoleSelector = (HtmlSelect)container.FindControl("roles");
            this._LoadingTag = (HtmlGenericControl)container.FindControl("loadingTag");
            this._RelativeLink = (HtmlAnchor)container.FindControl("relativeLink");

            HtmlGenericControl img = (HtmlGenericControl)container.FindControl("loadingImg");

            if (img != null)
                img.Style["background-image"] = string.Format("url({0})", ControlResources.HourglassLogoUrl);

            OguApplicationCollection apps = new OguApplicationCollection(PermissionMechanismFactory.GetMechanism().GetAllApplications());

            this._ApplicationsData = apps;

            if (apps.Count > 0)
            {
                string appCodeName = GetAppCodeNameFromFullCodeName(this.SelectedFullCodeName);

                IApplication selectedApp = null;

                if (string.IsNullOrEmpty(appCodeName) == false)
                    selectedApp = apps.Find(app => app.CodeName == appCodeName);

                if (selectedApp == null)
                    selectedApp = apps[0];

                this._RolesData = new OguRoleCollection(selectedApp.Roles);
            }
        }
        protected override void OnLoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
        {
            base.OnLoadingDialogContent(sender, e);
        }


        /*
         * 
         * TODO:这里，换了基类，所以暂时注掉
         * 
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
         * 
         *  * 
         * */


        protected override void LoadClientState(string clientState)
        {
            base.LoadClientState(clientState);

            if (string.IsNullOrEmpty(clientState) == false)
                this.SelectedFullCodeName = clientState;
        }

        /*
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
         * */

        private static OguApplicationCollection InitApplicationsData(string fullCodeName)
        {
            OguApplicationCollection result = null;

            string appCodeName = GetAppCodeNameFromFullCodeName(fullCodeName);

            if (string.IsNullOrEmpty(appCodeName))
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

            if (string.IsNullOrEmpty(fullCodeName) == false)
                result = fullCodeName.Split(':')[0];

            return result;
        }
    }
}
