using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.DelegationAuthorized
{
	public partial class AnyOriginalDelegationEdit : System.Web.UI.Page
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
			DelegateSrc.SelectedOuUserData.Add(OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, Data.SourceUserID)[0]);
		}

		[ControllerMethod]
		protected void DefaultProcessRequest(string sourceID, string destinationID)
		{
			sourceID.NullCheck("sourceID");
			destinationID.NullCheck("destinationID");

			{
				var delegation = WfDelegationAdapter.Instance.Load(builder =>
				{
					builder.AppendItem("SOURCE_USER_ID", sourceID);
					builder.AppendItem("DESTINATION_USER_ID", destinationID);
				}).FirstOrDefault();

				if (delegation != null)
				{
					this.Data = delegation;

					IOguObject src = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, Data.SourceUserID)[0];
					IOguObject dest = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, Data.DestinationUserID)[0];

					DelegateSrc.SelectedOuUserData.Add(src);
					DelegateDest.SelectedOuUserData.Add(dest);
				}
			}
		}

		protected void BtnSave_Click(object sender, EventArgs e)
		{
			try
			{
				this.bindingControl.CollectData();

				ValidateInput();

				Data.SourceUserID = DelegateSrc.SelectedOuUserData[0].ID;
				Data.SourceUserName = DelegateSrc.SelectedOuUserData[0].DisplayName;
				Data.DestinationUserName = DelegateDest.SelectedOuUserData[0].DisplayName;
				Data.DestinationUserID = DelegateDest.SelectedOuUserData[0].ID;
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
			ExceptionHelper.FalseThrow(DelegateSrc.SelectedOuUserData.Count > 0, "必须选择委托人");
			ExceptionHelper.FalseThrow(DelegateDest.SelectedOuUserData.Count > 0, "必须选择被委托人");
		}
	}
}