using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Operations;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace AUCenter.Dialogs
{
	[SceneUsage("~/App_Data/Editor.xml", "PropertyEdit")]
	public partial class Editor : System.Web.UI.Page, ITimeSceneDescriptor, INormalSceneDescriptor
	{
		private SchemaObjectBase Data;
		private bool sceneDirty = true;
		private bool enabled;
		private PropertyEditorSceneAdapter sceneAdapter;

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

		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.PropertyEditorRegister();

			if (this.IsPostBack == false && this.IsCallback == false)
				ControllerHelper.ExecuteMethodByRequest(this);
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			this.Data = SchemaExtensions.CreateObject(Request.Form["currentSchemaType"]);

			PropertyValueCollection pvc = JSONSerializerExecute.Deserialize<PropertyValueCollection>(Request.Form.GetValue("properties", string.Empty));

			this.Data.Properties.FromPropertyValues(pvc);

			if (this.Data is IPropertyExtendedObject)
			{
				((IPropertyExtendedObject)this.Data).EnsureExtendedProperties();
				this.Data.Properties.FromPropertyValues(pvc);
			}
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			PropertyEditorHelper.AttachToPage(this);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (this.IsPostBack == false && this.IsCallback == false)
			{
				this.RenderAllTabs(this.Data, this.EditEnabled == false || this.Data.Status != SchemaObjectStatus.Normal);
			}

			base.OnPreRender(e);

			this.description.InnerText = ObjectSchemaSettings.GetConfig().Schemas[this.Data.SchemaType].Description;
			this.lblCurrentID.InnerText = this.Data != null ? this.Data.ID : "(空)";
		}

		protected void SaveClick(object sender, EventArgs e)
		{
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ResetSubmitButton", "top.SubmitButton.resetAllStates();", true);

			if (string.IsNullOrWhiteSpace(this.alterKey.Text) == false)
			{
				this.Data.ID = this.alterKey.Text;
			}

			try
			{
				Util.EnsureOperationSafe();
				SchemaObjectBase parent = null;

				if (string.IsNullOrEmpty(this.currentParentID.Value) == false)
					AUCommon.DoDbAction(() =>
						parent = SchemaObjectAdapter.Instance.Load(this.currentParentID.Value));

				Facade.InstanceWithPermissions.DoOperation(this.OperationMode, this.Data, parent);

				WebUtility.ResponseCloseWindowScriptBlock();

				ScriptManager.RegisterClientScriptBlock(this.panelContainer, this.GetType(), "master", "top.window.close();", true);
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.GetRealException());
			}
		}

		#region ControllerMethods

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
		protected void CreateAUSchema(string category)
		{
			category.CheckStringIsNullOrEmpty("category");

			this.sceneAdapter = PropertyEditorSceneAdapter.Create(AUCommon.SchemaAUSchema);
			this.sceneAdapter.Mode = SCObjectOperationMode.Add;

			this.CreateNewObject(AUCommon.SchemaAUSchema);

			((AUSchema)this.Data).CategoryID = category;

			this.currentParentID.Value = null;
		}

		[ControllerMethod]
		protected void CreateAdminUnit(string auSchemaID, string parentID)
		{
			parentID = parentID == string.Empty ? null : parentID;

			if (string.IsNullOrEmpty(auSchemaID))
				throw new HttpException("auSchemaID不能为空");
			this.sceneAdapter = new AdminUnitsPropertyEditorSceneAdapter(DbUtil.GetEffectiveObject<AUSchema>(auSchemaID));
			this.sceneAdapter.ParentID = parentID;
			this.sceneAdapter.Mode = SCObjectOperationMode.Add;

			this.CreateNewObject(AUCommon.SchemaAdminUnit);
			((AdminUnit)this.Data).AUSchemaID = auSchemaID;

			this.currentParentID.Value = parentID;
		}

		[ControllerMethod]
		protected void LoadObject(string id)
		{
			AUCommon.DoDbAction(() =>
			{
				this.Data = SchemaObjectAdapter.Instance.Load(id);
			});

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
		#endregion

		private void CreateNewObjectBySchemaType(string schemaType)
		{
			this.Data = SchemaExtensions.CreateObject(schemaType);

			this.currentSchemaType.Value = schemaType;
			this.Data.ID = UuidHelper.NewUuidString();
		}

		private void RenderAllTabs(SchemaObjectBase data, bool readOnly)
		{
			Dictionary<string, SchemaPropertyValueCollection> tabGroup = data.Properties.GroupByTab();

			this.tabs.TabPages.Clear();

			foreach (SchemaTabDefine tab in data.Schema.Tabs)
			{
				SchemaPropertyValueCollection properties = null;

				if (tabGroup.TryGetValue(tab.Name, out properties) == false)
					properties = new SchemaPropertyValueCollection();

				this.RenderOnePanel(tab, properties, readOnly);
			}

			if (tabs.TabPages.Count > 0)
				tabs.ActiveTabPageIndex = 0;
		}

		private Control RenderOnePanel(SchemaTabDefine tab, SchemaPropertyValueCollection properties, bool readOnly)
		{
			RelaxedTabPage tabPage = new RelaxedTabPage()
			{
				Title = tab.Description,
				TagKey = tab.Name
			};

			this.tabs.TabPages.Add(tabPage);

			PropertyForm pForm = new PropertyForm() { AutoSaveClientState = false };
			pForm.ID = tab.Name + "_Form";

			//// if (currentScene.Items[this.tabStrip.ID].Recursive == true)
			////    pForm.ReadOnly = currentScene.Items[this.tabStrip.ID].ReadOnly;

			pForm.Properties.CopyFrom(properties.ToPropertyValues());

			PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
			layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

			pForm.Layouts.InitFromLayoutSectionCollection(layouts);

			pForm.Style["width"] = "100%";
			pForm.Style["height"] = "400";

			tabPage.Controls.Add(pForm);
			pForm.ReadOnly = readOnly;

			return tabPage;
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new AdminScopeEditor());
			PropertyEditorHelper.RegisterEditor(new CodeNameUniqueEditor());
			PropertyEditorHelper.RegisterEditor(new GetPinYinEditor());
			//PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new PObjectNameEditor());
			PropertyEditorHelper.RegisterEditor(new SchemaCategoryEditor());
			PropertyEditorHelper.RegisterEditor(new RoleGraphPropertyEditor());
		}
	}
}