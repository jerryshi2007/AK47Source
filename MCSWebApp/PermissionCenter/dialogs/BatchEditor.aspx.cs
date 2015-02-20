using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	[SceneUsage("~/App_Data/PropertyEditScene.xml", "PropertyEdit")]
	public partial class BatchEditor : System.Web.UI.Page, ITimeSceneDescriptor
	{
		private bool sceneDirty = true;
		private bool enabled = false;

		string ITimeSceneDescriptor.NormalSceneName
		{
			get { return this.EditEnabled ? "Normal" : "ReadOnly"; }
		}

		string ITimeSceneDescriptor.ReadOnlySceneName
		{
			get { return "ReadOnly"; }
		}

		#region 受保护的属性
		protected bool EditEnabled
		{
			get
			{
				if (this.sceneDirty)
				{
					this.enabled = TimePointContext.Current.UseCurrentTime;

					this.sceneDirty = false;
				}

				return this.enabled;
			}
		}
		#endregion

		public override void ProcessRequest(HttpContext context)
		{
			switch (context.Request.QueryString["action"])
			{
				case "request":
					context.Server.Execute("~/dialogs/BatchEditorIntro.htm", true);
					break;
				case "submit":
					ProcessSubmit(context);
					break;
				default:
					base.ProcessRequest(context);
					break;
			}
		}

		private void ProcessSubmit(HttpContext context)
		{
			string[] ids = context.Request.Form.GetValues("ids");
			string pptValue = context.Request.Form.Get("properties");
			var output = context.Response.Output;
			context.Response.Buffer = false;

			try
			{
				if (TimePointContext.Current.UseCurrentTime == false)
					throw new ApplicationException("只运行在“现在”时间上下文进行操作。");

				if (ids != null && pptValue != null)
				{
					PropertyValueCollection values = JSONSerializerExecute.Deserialize<PropertyValueCollection>(pptValue);

					var objects = DbUtil.LoadObjects(ids);
					for (int i = 0; i < ids.Length; i++)
					{
						ResponsePreparing(i, output);
						if (objects.ContainsKey(ids[i]) == false)
							ResponseNotFound(i, output);
						else
						{
							var obj = objects[ids[i]];
							try
							{
								foreach (var item in values)
								{
									obj.Properties[item.Definition.Name].StringValue = item.StringValue;
								}

								PC.Executors.SCObjectOperations.InstanceWithPermissions.DoOperation(SCObjectOperationMode.Update, obj, null);

								ResponseDone(i, output);
							}
							catch (Exception ex)
							{
								ResponseError(i, output, ex.Message, ex.StackTrace);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.GetRealException());
			}
			finally
			{
				ResponseFinished(output);
				output.Flush();
			}
		}

		private void ResponseFinished(TextWriter output)
		{
			output.WriteLine(Util.SurroundScriptBlock("top.setFinished();"));
		}

		private void ResponseError(int i, TextWriter output, string message, string stack)
		{
			message = JSONSerializerExecute.Serialize(message ?? string.Empty);
			stack = JSONSerializerExecute.Serialize(stack ?? string.Empty);
			output.WriteLine(Util.SurroundScriptBlock(string.Format("top.setError({0},{1},{2});", i.ToString(), message, stack)));
			output.Flush();
		}

		private void ResponseDone(int i, System.IO.TextWriter output)
		{
			output.WriteLine(Util.SurroundScriptBlock("top.setDone(" + i + ");"));
		}

		private void ResponseNotFound(int i, System.IO.TextWriter output)
		{
			output.WriteLine(Util.SurroundScriptBlock("top.setNotFound(" + i + ");"));
		}

		private void ResponsePreparing(int i, System.IO.TextWriter output)
		{
			output.WriteLine(Util.SurroundScriptBlock("top.setPreparing(" + i + ");"));
			output.Flush();
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			PropertyEditorHelper.AttachToPage(this);
			if (this.IsPostBack == false && this.IsCallback == false)
			{
				string[] ids = Request.Params.GetValues("ids");
				if (ids != null)
				{
					this.objNum.InnerText = ids.Length.ToString();
					var objects = DbUtil.LoadObjects(ids);
					HashSet<string> schemaTypes = GetSchemaTypes(objects);
					HashSet<string> groupNames = GetCommonGroups(schemaTypes);
					List<SchemaPropertyDefineConfigurationElement> propertyDefines = GetPropertyDefines(groupNames);
					SchemaPropertyValueCollection propertyValues = GetCommonValues(objects, propertyDefines);

					HashSet<string> tabNames = GetTabNames(propertyDefines);
					RenderTabs(schemaTypes, propertyValues, tabNames);

					this.ViewState["ids"] = ids;
					this.ViewState["schemaTypes"] = schemaTypes;
					this.objectDetails.Value = JSONSerializerExecute.Serialize(objects);
				}
			}
			else
			{
				HashSet<string> schemaTypes = this.ViewState["schemaTypes"] as HashSet<string>;
				if (schemaTypes != null)
				{
					HashSet<string> groupNames = GetCommonGroups(schemaTypes);
					List<SchemaPropertyDefineConfigurationElement> propertyDefines = GetPropertyDefines(groupNames);
				}
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

		}

		private void RenderTabs(HashSet<string> schemaTypes, SchemaPropertyValueCollection propertyValues, HashSet<string> tabNames)
		{
			if (tabNames.Count > 0)
			{
				var tabs = ObjectSchemaSettings.GetConfig().Schemas[schemaTypes.First()].Tabs;
				foreach (string item in tabNames)
				{
					RelaxedTabPage tabPage = new RelaxedTabPage()
					{
						Title = tabs[item].Description,
						TagKey = item
					};

					this.tabs.TabPages.Add(tabPage);

					PropertyForm pForm = new PropertyForm() { AutoSaveClientState = false };
					pForm.ID = tabPage.TagKey + "_Form";
					pForm.ReadOnly = this.EditEnabled == false;

					//// if (currentScene.Items[this.tabStrip.ID].Recursive == true)
					////    pForm.ReadOnly = currentScene.Items[this.tabStrip.ID].ReadOnly;

					var pageValues = TabGroup(propertyValues, item).ToPropertyValues();
					pForm.Properties.CopyFrom(pageValues);
					pForm.ShowCheckBoxes = true;

					PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
					layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

					pForm.Layouts.InitFromLayoutSectionCollection(layouts);

					pForm.Style["width"] = "100%";
					pForm.Style["height"] = "400";

					tabPage.Controls.Add(pForm);
				}

				this.tabs.ActiveTabPageIndex = 0;
			}
		}

		private SchemaPropertyValueCollection TabGroup(SchemaPropertyValueCollection propertyValues, string groupName)
		{
			SchemaPropertyValueCollection result = new SchemaPropertyValueCollection();
			foreach (var property in propertyValues)
			{
				if (property.Definition.Tab == groupName)
				{
					result.Add(property);
				}
			}

			return result;
		}

		private SchemaPropertyValueCollection GetCommonValues(SchemaObjectCollection objects, List<SchemaPropertyDefineConfigurationElement> propertyDefines)
		{
			SchemaPropertyValueCollection result = new SchemaPropertyValueCollection();

			HashSet<string> pool = new HashSet<string>();

			foreach (var item in propertyDefines)
			{
				pool.Clear();

				SchemaPropertyValue val = null;

				foreach (var obj in objects)
				{
					val = obj.Properties[item.Name];
					pool.Add(val.StringValue);
				}

				result.Add(pool.Count == 1 ? val : new SchemaPropertyValue(val.Definition));
			}

			return result;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Util.InitSecurityContext(this.notice);

			this.PropertyEditorRegister();
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

		private HashSet<string> GetTabNames(List<SchemaPropertyDefineConfigurationElement> propertyDefines)
		{
			HashSet<string> set = new HashSet<string>();
			foreach (var item in propertyDefines)
			{
				set.Add(item.Tab);
			}

			set.Remove(string.Empty); //移除没有Tab的项目

			return set;
		}

		private static List<SchemaPropertyDefineConfigurationElement> GetPropertyDefines(HashSet<string> groupNames)
		{
			List<SchemaPropertyDefineConfigurationElement> interests = new List<SchemaPropertyDefineConfigurationElement>();

			var settings = SchemaPropertyGroupSettings.GetConfig();
			foreach (var name in groupNames)
			{
				var grp = settings.Groups[name];
				if (grp != null)
				{
					foreach (SchemaPropertyDefineConfigurationElement ppt in grp.AllProperties)
					{
						if (ppt.BatchMode == PropertyBatchMode.Normal && ppt.ReadOnly == false)
						{
							interests.Add(ppt);
						}
					}
				}
			}
			return interests;
		}

		private HashSet<string> GetCommonGroups(HashSet<string> schemaTypes)
		{
			HashSet<string> groupNames = new HashSet<string>();
			List<string> tempNames = new List<string>();

			// 提取所有的组
			foreach (string item in schemaTypes)
			{
				var schemaConfig = ObjectSchemaSettings.GetConfig().Schemas[item];
				foreach (ObjectSchemaClassConfigurationElement elem in schemaConfig.Groups)
				{
					groupNames.Add(elem.GroupName);
				}
			}

			//保留公有的组
			foreach (string item in schemaTypes)
			{
				tempNames.Clear();
				var schemaConfig = ObjectSchemaSettings.GetConfig().Schemas[item];
				foreach (ObjectSchemaClassConfigurationElement elem in schemaConfig.Groups)
				{
					tempNames.Add(elem.GroupName);
				}

				groupNames.IntersectWith(tempNames);
			}
			return groupNames;
		}

		private HashSet<string> GetSchemaTypes(PC.SchemaObjectCollection objects)
		{
			HashSet<string> result = new HashSet<string>();
			objects.ForEach<PC.SchemaObjectBase>(item => result.Add(item.SchemaType));

			return result;
		}
	}
}