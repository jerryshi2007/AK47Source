using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.MVC;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.OA.CommonPages.UserMapping
{
    public partial class SetUserMappingRelation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!IsPostBack)
            {
                ControllerHelper.ExecuteMethodByRequest(this);
            }
        }


        [ControllerMethod]
        protected void SetGroupRequest(string extUserId,string mappingUserId)
        {
            if (!string.IsNullOrEmpty(mappingUserId))
            {
                IOguObject oguObject =
                         OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, mappingUserId)[
                             0];
                DelegatedUserInput.SelectedSingleData = oguObject;
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string extUserID = this.Request.QueryString["extUserId"];
                string mappingUserId = DelegatedUserInput.SelectedSingleData.ID;
                ExtUserMappingAdapter.Instance.SetMappingUser(extUserID,mappingUserId);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), ""
                    , "refreshParent();", true);
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }

        }
    }
}