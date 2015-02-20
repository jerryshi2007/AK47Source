using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Web.Library.MVC;
using System.Collections.Specialized;
using MCS.Library.SOA.DataObjects.Workflow;
using WfFormTemplate.DataObjects;

namespace WfFormTemplate.Forms
{
    /// <summary>
    /// 表单模版的前置控制器
    /// </summary>
    [ControllerNavigationTarget("", "DynamicFormView.aspx", "genericProcess")]
    [SceneInfoAttribute("../Scenes/DynamicFormProcess.xml", "DynamicFormProcess", ReadOnlySceneID = "ReadOnly", DefaultWorkflowSceneID = "Normal")]
    public class DynamicFormController : ControllerBase
    {
        protected override void OnAfterInitOperation(ControllerOperationBase operation)
        {
            operation.PrepareCommandState += new PrepareCommandStateHandler(Controller_PrepareCommandState);
        }

        private CommandStateBase Controller_PrepareCommandState(IWfProcess process)
        {
            DynamicFormCommandState state = null;

            //从流程上下文中获取数据。在这里通过流程上下文保存表单数据，省去了单独建表存储的工作
            //var data = DynamicFormDataAdapter.Instance.Load(process.ResourceID, false); //(string)process.RootProcess.Context["appData"];;

            string strData = (string)process.RootProcess.Context["appData"];
            
            if (strData.IsNullOrEmpty())
            {
                DynamicFormData data = new DynamicFormData();
            
                //data.Properties = null;

                state = new DynamicFormCommandState() { Data = data };

                process.GenerateCandidatesFromResources();
            }
            else
            {
                var data = SerializationHelper.DeserializeStringToObject(strData, SerializationFormatterType.Binary) as DynamicFormData;
                return new DynamicFormCommandState() { Data = data };
            }

            return state;
        }
    }
}