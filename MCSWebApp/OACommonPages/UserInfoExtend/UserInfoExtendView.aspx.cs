using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.MVC;

namespace MCS.OA.CommonPages.UserInfoExtend
{
    public partial class UserInfoExtendView : System.Web.UI.Page
    {
        private class AppRoles
        {
            public string AppName { get; set; }
            public string Roles { get; set; }
        }

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
            OguObjectCollection<IUser> user =
                OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.FullPath, fpath);

            foreach (IUser userObj in user)
            {
                UserInfoExtendCollection collection = UserInfoExtendDataObjectAdapter.Instance.Load(builder => builder.AppendItem("ID", userObj.ID));
                if (collection.Count > 0) Data = collection[0];
                if (collection.Count == 0) Data = new UserInfoExtendDataObject(){ID =userObj.ID};
                Data.DisplayName =userObj.DisplayName;
                Data.FullPath = userObj.FullPath;
                Data.LogonName = userObj.LogOnName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();
            if (!IsPostBack)
            {
                ControllerHelper.ExecuteMethodByRequest(this);
                InitPageVar();
            }
            this.bindingControl.Data = Data;
        }

        protected override void OnPreRender(EventArgs e)
        {
            GetApplicationRole();

            base.OnPreRender(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (Data.Birthday == DateTime.MinValue)
            {
                labBirthday.ReadOnly = false;
                labBirthday.Text = "";
                labBirthday.ReadOnly = true;
            }

            if (Data.StartWorkTime == DateTime.MinValue)
            {
                labWorkTime.ReadOnly = false;
                labWorkTime.Text = "";
                labWorkTime.ReadOnly = true;
            }

            base.OnPreRenderComplete(e);
        }

        private void InitPageVar()
        {
            BindData(Data.FullPath);
        }

        private void BindData(string fullPath)
        {
            string fPath = HttpUtility.UrlEncode(fullPath);
            btnUpdate.Attributes["onclick"] = "onUpdateClick('" + fPath + "')";
        }

        private void GetApplicationRole()
        {
            List<AppRoles> allRoles = new List<AppRoles>();

            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, Data.ID);

            ExceptionHelper.FalseThrow(users.Count > 0, "不能找到ID为\"{0}\"的用户", Data.ID);

            ApplicationCollection result = PermissionMechanismFactory.GetMechanism().GetAllApplications();

            foreach (IApplication app in result)
            {
                RoleCollection roles = users[0].Roles[app.CodeName];
                if (roles.Count > 0)
                {
                    AppRoles ar = new AppRoles();

                    ar.AppName = app.Name;
                    ar.Roles = GetRoleNames(roles);

                    allRoles.Add(ar);
                }
            }

            gridAppRoles.DataSource = allRoles;
            gridAppRoles.DataBind();
        }

        private static string GetRoleNames(RoleCollection roles)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < roles.Count; i++)
            {
                if (i != 0)
                    strB.Append(", ");

                strB.Append(roles[i].Name);
            }

            return strB.ToString();
        }
    }
}