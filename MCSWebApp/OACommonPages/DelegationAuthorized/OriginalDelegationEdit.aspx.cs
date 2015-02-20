using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.OA.CommonPages.DelegationAuthorized
{
    public partial class OriginalDelegationEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (!IsPostBack)
                ControllerHelper.ExecuteMethodByRequest(this);
            if (Data != null)
                bindingControl.Data = Data;
                

        }

        public WfDelegation Data
        {
            get { return (WfDelegation)ViewState["Data"]; }
            set { ViewState["Data"] = value; }
        }

        [ControllerMethod(true)]
        protected void DefaultProcessRequest()
        {
            Data = new WfDelegation();
            Data.SourceUserID = DeluxeIdentity.CurrentUser.ID;
            Data.SourceUserName = DeluxeIdentity.CurrentUser.Name;
        }

        [ControllerMethod]
        protected void DefaultProcessRequest(string delegateUserID)
        {
            if (string.IsNullOrEmpty(delegateUserID))
                DefaultProcessRequest();
            else
            {
                WfDelegationCollection delegation = WfDelegationAdapter.Instance.Load(builder => builder.AppendItem("DESTINATION_USER_ID", delegateUserID));
                WfDelegation wfDelegation = new WfDelegation();
                foreach (WfDelegation delegation1 in delegation)
                {
                    wfDelegation.DestinationUserID = delegation1.DestinationUserID;
                    wfDelegation.DestinationUserName = delegation1.DestinationUserName;
                    wfDelegation.StartTime = delegation1.StartTime;
                    wfDelegation.EndTime = delegation1.EndTime;
                    wfDelegation.SourceUserID = delegation1.SourceUserID;
                    wfDelegation.SourceUserName = delegation1.SourceUserName;
                }
                if (delegation.Count > 0)
                {
                    Data = wfDelegation;
                    IOguObject oguObject =
                        OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, Data.DestinationUserID)[
                            0];
                    DelegatedUserInput.SelectedOuUserData.Add(oguObject);
                }
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.bindingControl.CollectData();

                ValidateInput();

                Data.DestinationUserName = DelegatedUserInput.SelectedOuUserData[0].DisplayName;
                Data.DestinationUserID = DelegatedUserInput.SelectedOuUserData[0].ID;
                WfDelegationAdapter.Instance.Update(Data);

				LogUtil.AppendLogToDb(LogUtil.CreateAssignLog(this.Data));

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), ""
                    , "refreshParent();", true);
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }

        }

        private void ValidateInput()
        {
            ExceptionHelper.FalseThrow(Data.StartTime != DateTime.MinValue && Data.EndTime != DateTime.MinValue, "必须填写开始时间和结束时间");
            ExceptionHelper.FalseThrow(Data.StartTime <= Data.EndTime, "结束时间不能小于开始时间");

            ExceptionHelper.FalseThrow(DelegatedUserInput.SelectedOuUserData.Count > 0, "必须选择被委托人");
        }

    }
}