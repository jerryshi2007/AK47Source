using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.MVC;
using System.Collections.Specialized;
using MCS.Library.SOA.DataObjects.Workflow;
using WfFormTemplate.DataObjects;

namespace WfFormTemplate.Forms
{
    /// <summary>
    /// 表单模版的前置控制器
    /// </summary>
    [ControllerNavigationTarget("", "TemplateView.aspx", "genericProcess")]
    [SceneInfoAttribute("../Scenes/TempProcess.xml", "TempProcess", ReadOnlySceneID = "ReadOnly", DefaultWorkflowSceneID = "Normal")]
    public class TemplateController : ControllerBase
    {
        /// <summary>
        /// 如果URL中没有参数，则自动转到选人的界面，动态构造流程
        /// </summary>
        protected override void DefaultOperation()
        {
            TransferCommand command = new TransferCommand("SelectUser");

            HttpRequest request = HttpContext.Current.Request;

            NameValueCollection uriParams = UriHelper.GetUriParamsCollection(request.Url.ToString());

            command.NavigateUrl = string.Format("SelectProcessUsers.aspx?ru={0}",
                HttpUtility.UrlEncode(UriHelper.CombineUrlParams(request.CurrentExecutionFilePath, uriParams)));

            command.Execute();
        }

        protected override void OnAfterInitOperation(ControllerOperationBase operation)
        {
            operation.PrepareCommandState += new PrepareCommandStateHandler(Controller_PrepareCommandState);
        }

        private CommandStateBase Controller_PrepareCommandState(IWfProcess process)
        {
            TemplateCommandState state = null;

            //从流程上下文中获取数据。在这里通过流程上下文保存表单数据，省去了单独建表存储的工作
            string appData = (string)process.RootProcess.Context["appData"];

            if (appData.IsNullOrEmpty())
            {
                TemplateData data = new TemplateData();
                data.ID = process.ResourceID;

                data.CostCenter = "1001";
                data.AdministrativeUnit = "Group";

                state = new TemplateCommandState() { Data = data };

                process.ApplicationRuntimeParameters["Amount"] = data.Amount;
                process.ApplicationRuntimeParameters["CostCenter"] = data.CostCenter;
                process.ApplicationRuntimeParameters["AdministrativeUnit"] = data.AdministrativeUnit;
                process.ApplicationRuntimeParameters["TempApprover"] =
                    (IUser)OguUser.CreateWrapperObject(OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "fanhy").First());

                process.GenerateCandidatesFromResources();
            }
            else
            {
                state = (TemplateCommandState)SerializationHelper.DeserializeStringToObject(appData, SerializationFormatterType.Binary);
                state.Data.Loaded = true;
            }

            return state;
        }
    }
}