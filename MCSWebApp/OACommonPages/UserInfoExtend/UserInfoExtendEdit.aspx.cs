using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.UserInfoExtend
{
    public partial class UserInfoExtendEdit : System.Web.UI.Page
    {
        private UserInfoExtendDataObject Data
        {
            get
            {
                return (UserInfoExtendDataObject)ViewState["Data"];
            }
            set
            {
                ViewState["Data"] = value;
            }
        }

        [ControllerMethod]
        protected void DefaultProcessRequest(string fpath)
        {
            OguObjectCollection<IUser> user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.FullPath, fpath);

            foreach (IUser user1 in user)
            {
                UserInfoExtendCollection extendCollection = UserInfoExtendDataObjectAdapter.Instance.Load(builder => builder.AppendItem("ID", user1.ID));
                if (extendCollection.Count > 0) Data = extendCollection[0];
                if (extendCollection.Count == 0) Data = new UserInfoExtendDataObject() { ID = user1.ID };
                Data.DisplayName = user1.DisplayName;
                Data.FullPath = user1.FullPath;
                Data.LogonName = user1.LogOnName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();
            if (!IsPostBack)
            {
                ControllerHelper.ExecuteMethodByRequest(this);
            }
            this.bindingControl.Data = Data;


        }

        protected void submitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.bindingControl.CollectData();

                UserInfoExtendDataObjectAdapter.Instance.Update(this.Data);

                WebUtility.ResponseRefreshParentWindowScriptBlock();

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "UserInfoExtendEdit",
                    "top.location.href='UserInfoExtendView.aspx?fpath=" + HttpUtility.UrlEncode(Data.FullPath) + "'", true);
            }
            catch (System.Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }
    }
}