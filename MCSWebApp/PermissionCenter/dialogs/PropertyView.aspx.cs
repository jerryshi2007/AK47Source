using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.Dialogs
{
	public partial class PropertyView : System.Web.UI.Page
	{
		#region 私有属性

		private SchemaObjectBase Data
		{
			get;
			set;
		}

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
				} */

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
			if (this.IsPostBack == false && this.IsCallback == false)
				ControllerHelper.ExecuteMethodByRequest(this);

			this.PropertyEditorRegister();
		}

		[ControllerMethod]
		protected void LoadObject(string id)
		{
			this.Data = SchemaObjectAdapter.Instance.Load(id);

			this.currentSchemaType.Value = this.Data.SchemaType;
			this.OperationMode = SCObjectOperationMode.Update;
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			PropertyEditorHelper.AttachToPage(this);

			Response.Cache.SetCacheability(HttpCacheability.NoCache);
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
			Act act = ActHelper.GetActs("~/App_Data/PropertyEditScene.xml")["propertyEdit"];

			Scene currentScene = act.Scenes[TimePointContext.Current.UseCurrentTime ? "Normal" : "ReadOnly"];

			this.RenderAllTabs(this.Data, currentScene);

			Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			currentScene.RenderToControl(this);

			base.OnPreRender(e);
		}

		[ControllerMethod(true)]
		protected void CreateNewObject()
		{
			this.CreateNewObject("Users");
		}

		[ControllerMethod]
		protected void CreateNewObject(string schemaType)
		{
			this.CreateNewObjectBySchemaType(schemaType);

			this.OperationMode = SCObjectOperationMode.Add;
		}

		[ControllerMethod]
		protected void CreateNewObject(string schemaType, string parentID)
		{
			this.CreateNewObject(schemaType);

			this.ParentID = parentID;
		}

		protected void Save_Click(object sender, EventArgs e)
		{
			try
			{
				Util.EnsureOperationSafe();
				SchemaObjectBase parent = null;

				if (this.ParentID.IsNotEmpty())
					parent = SchemaObjectAdapter.Instance.Load(this.ParentID);

				using (var scope = TransactionScopeFactory.Create())
				{
					this.Data.Properties.Write();

					SCObjectOperations.InstanceWithPermissions.DoOperation(this.OperationMode, this.Data, parent);

					scope.Complete();
				}

				// 等待全文目录更新
				WebUtility.ResponseShowClientMessageScriptBlock("可能需要几秒钟之后重新查询列表才会反映所做的更改。", string.Empty, "保存成功");

				WebUtility.ResponseCloseWindowScriptBlock();
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
			this.currentSchemaType.Value = schemaType;
			this.Data.ID = UuidHelper.NewUuidString();
		}

		private void RenderAllTabs(SchemaObjectBase data, Scene currentScene)
		{
			string defaultKey = string.Empty; // this.tabStrip.SelectedKey;

			Dictionary<string, SchemaPropertyValueCollection> tabGroup = data.Properties.GroupByTab();

			this.tabStrip.TabPages.Clear();

			foreach (SchemaTabDefine tab in data.Schema.Tabs)
			{
				SchemaPropertyValueCollection properties = null;

				if (tabGroup.TryGetValue(tab.Name, out properties) == false)
					properties = new SchemaPropertyValueCollection();

				Control panel = this.RenderOnePanel(tab, properties, currentScene);

				RelaxedTabPage item = new RelaxedTabPage()
				{
					Title = tab.Description,
				};

				item.Controls.Add(panel);

				this.tabStrip.TabPages.Add(item);
			}

			if (this.tabStrip.TabPages.Count > 0)
			{
				this.tabStrip.ActiveTabPageIndex = 0;
			}
		}

		private Control RenderOnePanel(SchemaTabDefine tab, SchemaPropertyValueCollection properties, Scene currentScene)
		{
			HtmlGenericControl panel = new HtmlGenericControl("div");

			panel.ID = tab.Name;
			panel.Style["width"] = "100%";
			panel.Style["height"] = "100%";

			PropertyForm pForm = new PropertyForm() { AutoSaveClientState = false };
			pForm.ID = tab.Name + "_Form";
			if (currentScene.Items[this.tabStrip.ID].Recursive == true)
				pForm.ReadOnly = currentScene.Items[this.tabStrip.ID].ReadOnly;

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
		}
		#endregion

		#endregion
	}
}