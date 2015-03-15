using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using System.Xml.Linq;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Web.WebControls;
using MCS.Web.WebControls.Configuration;

namespace WorkflowDesigner
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.xapPath.Attributes["value"] = WfRuntimeViewerSettings.GetConfig().DesignerXapUrl;
            this.enableSimulation.Text = "<param name=\"InitParams\" value=\"enableSimulation=" + WfSimulationSettings.GetConfig().Enabled.ToString() + "\" />";

            PropertyEditorRegister();

            InitializeTemplate();

            //InitializeEnumStore();

            if (Request["processid"].IsNotEmpty())
            {
                InitInstanceDescription(Request["processid"]);
            }
            else
                if (Request["processDescKey"].IsNotEmpty())
                {
                    InitInstanceByProcessDescKey(Request["processDescKey"]);
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
            PropertyEditorHelper.RegisterEditor(new ObjectListPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new ReceiversObjectListPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new ConditionExpressionPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new KeyPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new CanActivityKeysEditor());
            PropertyEditorHelper.RegisterEditor(new BranchProcessKeyPropertyEditor());
            PropertyEditorHelper.RegisterEditor(new DynamicPropertyEditor());
            //PropertyEditorHelper.RegisterEditor(new DefaultProcessKeyPropertyEditor());
        }

        protected override void OnPreRender(EventArgs e)
        {
            InitializeEnumStore();
            base.OnPreRender(e);
        }

        /// <summary>
        /// register enums used in PropertyGrid control
        /// </summary>
        private void InitializeEnumStore()
        {
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfProcessType, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfNavigatorDisplayMode, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfSubProcessApprovalMode, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchGroupBlockingType, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessBlockingType, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfBranchProcessExecuteSequence, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("WorkflowDesigner.PropertyEditor.BranchProcessKey, WorkflowDesigner");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfSearchIDMode, MCS.Library.SOA.DataObjects");
            this.propertyGrid.PredefinedEnumTypes.Add("MCS.Library.SOA.DataObjects.Workflow.WfAutoSendUserTaskMode, MCS.Library.SOA.DataObjects");
        }

        private static Dictionary<string, string> GetEnumDescriptionDictionary(Type enumType)
        {
            EnumItemDescriptionList enumList = EnumItemDescriptionAttribute.GetDescriptionList(enumType);

            Dictionary<string, string> result = new Dictionary<string, string>();

            enumList.ForEach(p => result.Add(p.Name, p.Description));

            return result;
        }

        private void InitializeTemplate()
        {
            this.processTemplate.Value = WfConverterHelper.GetEmptyProcessDescriptorJsonString();
            this.initActTemplate.Value = WfConverterHelper.GetEmptyInitialActivityDescriptorJsonString();
            this.normalActTemplate.Value = WfConverterHelper.GetEmptyNormalActivityDescriptorJsonString();
            this.completedActTemplate.Value = WfConverterHelper.GetEmptyCompletedActivityDescriptorJsonString();
            this.tranTemplate.Value = WfConverterHelper.GetEmptyForwardTransitionDescriptorJsonString();

            var templateCollection = WfActivityTemplateAdpter.Instance.LoadAvailableTemplates();
            StringBuilder strBuilder = new StringBuilder();
            templateCollection.ForEach(p => strBuilder.Append(p.Content + ","));

            if (strBuilder.Length > 0)
            {
                strBuilder.Remove(strBuilder.Length - 1, 1);
            }

            this.actUserTemplate.Value = string.Format("[{0}]", strBuilder.ToString());
        }

        private void InitInstanceDescription(string processID)
        {
            IWfProcess process = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessByProcessID(processID);
            if (process != null)
            {
                instanceDescription.Value = JSONSerializerExecute.Serialize(new List<IWfProcessDescriptor>() { process.Descriptor });
                this.processID.Value = processID;
            }
        }

        private void InitInstanceByProcessDescKey(string processDescKey)
        {
            List<IWfProcessDescriptor> processDesps = new List<IWfProcessDescriptor>();

            processDesps.Add(WfProcessDescriptorManager.LoadDescriptor(processDescKey));

            instanceDescription.Value = JSONSerializerExecute.Serialize(processDesps);
        }
    }
}