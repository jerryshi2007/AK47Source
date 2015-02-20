using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;

namespace PermissionCenter.Dialogs
{
    [SceneUsage("~/App_Data/PropertyEditScene.xml", "PropertyEdit")]
    public partial class ShowObjectInfo : System.Web.UI.Page, ITimeSceneDescriptor, INormalSceneDescriptor
    {
        private bool sceneDirty = true;
        private bool enabled = false;
        private PropertyEditorSceneAdapter sceneAdapter = null;

        string ITimeSceneDescriptor.NormalSceneName
        {
            get { return this.EditEnabled ? "Normal" : "ReadOnly"; }
        }

        string ITimeSceneDescriptor.ReadOnlySceneName
        {
            get { return "ReadOnly"; }
        }


        public void AfterNormalSceneApplied()
        {
            this.okButton.Visible = this.Data != null && this.Data.Status == SchemaObjectStatus.Normal;
        }

        #region 受保护的属性
        protected bool EditEnabled
        {
            get
            {
                if (this.sceneDirty)
                {
                    this.enabled = TimePointContext.Current.UseCurrentTime;

                    if (this.enabled && Util.SuperVisiorMode == false && this.sceneAdapter != null)
                    {
                        this.enabled = this.sceneAdapter.IsEditable();
                    }

                    this.sceneDirty = false;
                }

                return this.enabled;
            }
        }
        #endregion

        #region 私有的属性

        /* 因为是在前端需要用到 SchemaType 调用服务 
		private string CurrentSchemaType
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "CurrentSchemaType", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "CurrentSchemaType", value);
			}
		} 
		 
		private string ParentID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "ParentID", string.Empty);
			}

			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "ParentID", value);
			}
		}
		 */

        private SchemaObjectBase Data
        {
            get;
            set;
        }

        private SCObjectOperationMode OperationMode
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "OperationMode", SCObjectOperationMode.Add);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "OperationMode", value);
            }
        }

        #endregion

        #region 受保护的方法

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.InitSecurityContext(this.notice);

            if (this.IsPostBack == false && this.IsCallback == false)
                ControllerHelper.ExecuteMethodByRequest(this);

            this.PropertyEditorRegister();
        }

        [ControllerMethod(true)]
        protected void CreateNewObject()
        {
            this.CreateNewObject("Users");
        }

        [ControllerMethod]
        protected void CreateNewObject(string schemaType)
        {
            if (this.sceneAdapter == null)
            {
                this.sceneAdapter = PropertyEditorSceneAdapter.Create(schemaType);
                this.sceneAdapter.Mode = SCObjectOperationMode.Add;
            }

            this.CreateNewObjectBySchemaType(schemaType);

            this.OperationMode = SCObjectOperationMode.Add;
        }

        [ControllerMethod]
        protected void CreateNewObject(string schemaType, string parentID)
        {
            this.sceneAdapter = PropertyEditorSceneAdapter.Create(schemaType);
            this.sceneAdapter.ParentID = parentID;
            this.sceneAdapter.Mode = SCObjectOperationMode.Add;

            this.CreateNewObject(schemaType);

            this.currentParentID.Value = parentID;
        }

        [ControllerMethod]
        protected void LoadObject(string id)
        {
            this.Data = SchemaObjectAdapter.Instance.Load(id);

            this.sceneAdapter = PropertyEditorSceneAdapter.Create(this.Data.SchemaType);
            this.sceneAdapter.ObjectID = this.Data.ID;
            this.sceneAdapter.Mode = SCObjectOperationMode.Update;

            this.currentSchemaType.Value = this.Data.SchemaType;

            // this.CurrentSchemaType = this.Data.SchemaType;
            this.OperationMode = SCObjectOperationMode.Update;
        }

        [ControllerMethod]
        protected void LoadObject(string id, long reserved, string time)
        {
            TimePointContext.Current.UseCurrentTime = false;
            TimePointContext.Current.SimulatedTime = DateTime.Parse(time).ToLocalTime();
            this.LoadObject(id);
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PropertyEditorHelper.AttachToPage(this);
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            this.CreateNewObjectBySchemaType(Request.Form.GetValue("currentSchemaType", string.Empty));

            PropertyValueCollection pvc = JSONSerializerExecute.Deserialize<PropertyValueCollection>(Request.Form.GetValue("properties", string.Empty));

            this.Data.Properties.FromPropertyValues(pvc);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (this.IsPostBack == false && this.IsCallback == false)
            {
                this.RenderAllTabs(this.Data, this.EditEnabled == false);
            }

            base.OnPreRender(e);

            if (this.Data.Status != SchemaObjectStatus.Normal)
                this.okButton.Visible = false;
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ResetSubmitButton", "top.SubmitButton.resetAllStates();", true);

            try
            {
                Util.EnsureOperationSafe();
                SchemaObjectBase parent = null;

                if (this.currentParentID.Value.IsNotEmpty())
                    parent = SchemaObjectAdapter.Instance.Load(this.currentParentID.Value);

                SCObjectOperations.InstanceWithPermissions.DoOperation(this.OperationMode, this.Data, parent);

                SchemaObjectPhotoKey photoCacheKey = new SchemaObjectPhotoKey() { ObjectID = this.Data.ID, PropertyName = "PhotoKey", TimePoint = DateTime.MinValue };

                CacheNotifyData notifyData = new CacheNotifyData(typeof(SchemaObjectPhotoCache), photoCacheKey, CacheNotifyType.Invalid);

                UdpCacheNotifier.Instance.SendNotify(notifyData);
                MmfCacheNotifier.Instance.SendNotify(notifyData);

                WebUtility.ResponseCloseWindowScriptBlock();

                //ScriptManager.RegisterClientScriptBlock(this.panelContainer, this.GetType(), "master", "top.window.close();", true);
            }
            catch (System.Exception ex)
            {
                WebUtility.ShowClientError(ex.GetRealException());
            }
        }
        #endregion

        #region 私有的方法

        private void CreateNewObjectBySchemaType(string schemaType)
        {
            this.Data = SchemaExtensions.CreateObject(schemaType);

            // this.CurrentSchemaType = category;
            this.currentSchemaType.Value = schemaType;
            this.Data.ID = UuidHelper.NewUuidString();
        }

        private void RenderAllTabs(SchemaObjectBase data, bool readOnly)
        {
            string defaultKey = this.tabStrip.SelectedKey;

            Dictionary<string, SchemaPropertyValueCollection> tabGroup = data.Properties.GroupByTab();

            this.tabStrip.TabStrips.Clear();

            foreach (SchemaTabDefine tab in data.Schema.Tabs)
            {
                SchemaPropertyValueCollection properties = null;

                if (tabGroup.TryGetValue(tab.Name, out properties) == false)
                    properties = new SchemaPropertyValueCollection();

                Control panel = this.RenderOnePanel(tab, this.panelContainer, properties, readOnly);

                TabStripItem item = new TabStripItem() { Key = tab.Name, Text = tab.Description, ControlID = panel.ClientID, Tag = panel.Controls[0].ClientID };

                this.tabStrip.TabStrips.Add(item);

                if (defaultKey.IsNullOrEmpty())
                    defaultKey = item.Key;
            }

            if (defaultKey.IsNotEmpty())
                this.tabStrip.SelectedKey = defaultKey;
        }

        private Control RenderOnePanel(SchemaTabDefine tab, Control container, SchemaPropertyValueCollection properties, bool readOnly)
        {
            HtmlGenericControl panel = new HtmlGenericControl("div");

            panel.ID = tab.Name;
            panel.Style["width"] = "100%";
            panel.Style["height"] = "100%";

            this.panelContainer.Controls.Add(panel);

            PropertyForm pForm = new PropertyForm() { AutoSaveClientState = false };
            pForm.ID = tab.Name + "_Form";
            pForm.ReadOnly = readOnly;

            //// if (currentScene.Items[this.tabStrip.ID].Recursive == true)
            ////    pForm.ReadOnly = currentScene.Items[this.tabStrip.ID].ReadOnly;

            pForm.Properties.CopyFrom(properties.ToPropertyValues());

            PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
            layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

            pForm.Layouts.InitFromLayoutSectionCollection(layouts);

            pForm.Style["width"] = "100%";
            pForm.Style["height"] = "400";

            panel.Controls.Add(pForm);

            return panel;
        }

        #region "将来添加自定义PropertyEditor时需要在此注册"
        private void PropertyEditorRegister()
        {
            PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
            PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
            PropertyEditorHelper.RegisterEditor(new CodeNameUniqueEditor());
            PropertyEditorHelper.RegisterEditor(new GetPinYinEditor());
            PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new PObjectNameEditor());
        }
        #endregion

        #endregion
    }
}