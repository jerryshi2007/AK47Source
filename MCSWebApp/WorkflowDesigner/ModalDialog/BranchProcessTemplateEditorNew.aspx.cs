using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;

namespace WorkflowDesigner.ModalDialog
{
	public partial class BranchProcessTemplateEditorNew : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				PropertyEditorRegister();

				WfConverterHelper.RegisterConverters();
				WfBranchProcessTemplateDescriptor branchProcessTempDesc = new WfBranchProcessTemplateDescriptor(string.Empty);
				//branchProcessTempDesc.BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;
				//branchProcessTempDesc.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;
				this.hiddenBranchProcessTemplate.Value = JSONSerializerExecute.Serialize(branchProcessTempDesc);
				this.hiddenKey.Value = branchProcessTempDesc.Key;

				#region "用hidden properties 初始化没有属性值"
				//PropertyDefineCollection propetydefines = PropertyDefineCollection.CreatePropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["BasicBranchProcessTemplateProperties"]);
				//PropertyValueCollection properties = new PropertyValueCollection();
				//properties.InitFromPropertyDefineCollection(propetydefines);
				//this.hiddenBrnachConfigProperties.Value = JSONSerializerExecute.Serialize(properties);
				#endregion
			}
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new GenerateTypePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BranchProcessKeyPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ReceiversObjectListPropertyEditor());
			//PropertyEditorHelper.RegisterEditor(new BranchConditionPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ServiceOperationPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ConditionExpressionPropertyEditor());
		}

		protected override void OnPreRender(EventArgs e)
		{
			InitializeEnumStore();
			base.OnPreRender(e);
		}

		private void InitializeEnumStore()
		{
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessBlockingType", "MCS.Library.SOA.DataObjects"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessExecuteSequence", "MCS.Library.SOA.DataObjects"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfSubProcessResourceMode", "MCS.Library.SOA.DataObjects"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfSubProcessApprovalMode", "MCS.Library.SOA.DataObjects"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "WorkflowDesigner.PropertyEditor.BranchProcessKey", "WorkflowDesigner"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfOpinionMode", "MCS.Library.SOA.DataObjects"));
			this.propertyGrid.PredefinedEnumTypes.Add(string.Format("{0}, {1}", "MCS.Library.SOA.DataObjects.Workflow.WfSubProcessActivityEditMode", "MCS.Library.SOA.DataObjects"));
		}
	}
}