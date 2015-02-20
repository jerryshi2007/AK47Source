using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using MCS.Library.OGUPermission;
using System.Collections;
using MCS.Library.Core;
using System.Data.SqlClient;

namespace MCS.OA.CommonPages.WFGroup
{
    public partial class WFGroupEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!IsPostBack)
            {
                ControllerHelper.ExecuteMethodByRequest(this);
            }
            bindingControl.Data = Data;
        }

        public WfGroup Data
        {
            get { return (WfGroup)ViewState["Data"]; }
            set { ViewState["Data"] = value; }
        }
        public int LastQueryRowCount
        {
            get { return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1); }
            set { WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value); }
        }

        #region REQUEST PORCESSORS

        [ControllerMethod(true)]
        protected void NewGroupRequest()
        {
            //如果新增
            if (Data == null)
            {
                Data = new WfGroup();
                Data.GroupID = string.Empty;
                ExecQuery();
            }
        }

        [ControllerMethod]
        protected void EditGroupRequest(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                NewGroupRequest();
            }
            else
            {
                WfGroupCollection groups = WfGroupAdapter.Instance.Load(builder => builder.AppendItem("GROUP_ID", groupId));
                ExceptionHelper.FalseThrow(groups.Count > 0, "没有找到对应的群组定义");
                Data = groups[0];
                this.OuUserInputControlManager.SelectedSingleData = Data.Manager;
                ExecQuery();
            }
        }

        #endregion

        #region  Update Or Insert  GROUP

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(Data.GroupName))
            {
                return false;
            }
            return true;
        }
     
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            DeluxeIdentity identity = HttpContext.Current.User.Identity as DeluxeIdentity;
            ExceptionHelper.TrueThrow(identity == null, "无法获取登陆用户");
            bool newGroup = string.IsNullOrEmpty(Data.GroupID);
            try
            {              
                this.bindingControl.CollectData();
                IUser mangerUser = this.OuUserInputControlManager.SelectedSingleData as IUser;
                //新增组
                if (newGroup)
                {
                    Data.GroupID = Guid.NewGuid().ToString();
                    Data.CreateTime = System.DateTime.Now;
                    Data.Creator = identity.User;
                    Data.Manager = mangerUser;
                    WfGroupAdapter.Instance.Update(Data);

                    //把开始设置的负责人，自动添加到用户组内.
                    if (mangerUser != null)
                    {
                        IList<IUser> users = new List<IUser>();
                        users.Add(mangerUser);
                        WfGroupAdapter.Instance.AddGroupUsers(this.Data, users);
                        RefreshGrid();
                    }
                }
                else
                {
                    if (mangerUser != null)
                    {
                        //检查所设置的群组负责人是否有效
                        if (WfGroupAdapter.Instance.CheckGroupManager(this.Data, mangerUser))
                        {
                            Data.Manager = this.OuUserInputControlManager.SelectedSingleData as IUser;
                            WfGroupAdapter.Instance.Update(Data);
                        }
                        else
                        {
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "",
                                "alert('群组负责人必须为当前组内用户！');", true);
                            return;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (newGroup)
                {
                    Data.GroupID = string.Empty;
                }
                WebUtility.ShowClientError(sqlEx.Message, sqlEx.StackTrace, "错误");
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        #endregion

        #region ADD USER

        protected void BtnAddUser_Click(object sender, EventArgs e)
        {
            #region validate
            
            if (this.groupUserInput.SelectedOuUserData.Count == 0)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "alert('请选择要添加的用户！');",true);
                return;
            }
            if (string.IsNullOrEmpty(this.Data.GroupID))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "alert('请先保存群组！');",true);
                return;
            }

            #endregion

            this.bindingControl.CollectData();
            IList<IUser> users = new List<IUser>();
            for (int i = 0; i < this.groupUserInput.SelectedOuUserData.Count; i++)
            {
                users.Add(this.groupUserInput.SelectedOuUserData[i] as IUser);
            }
            WfGroupAdapter.Instance.AddGroupUsers(this.Data, users);
            this.groupUserInput.SelectedOuUserData.Clear();
            RefreshGrid();

        }

        #endregion       

        #region DELETE USER

        //protected void DeluxeGridGroupUser_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    //删除群组中的用户;
        //    if (e.CommandName == "DeleteGroupUser")
        //    {
        //        if (this.data != null && !string.IsNullOrEmpty(this.data.GroupID))
        //        {
        //            WFGroupAdapter.Instance.DeleteGroupUsers(this.data, e.CommandArgument.ToString());
        //        }
        //    }
        //}

        protected void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (this.Data != null && !string.IsNullOrEmpty(this.Data.GroupID)
                 && this.DeluxeGridGroupUser.SelectedKeys.Count>0)
            {
                string[] paramsUsers = new string[this.DeluxeGridGroupUser.SelectedKeys.Count];
                for (int i = 0; i < this.DeluxeGridGroupUser.SelectedKeys.Count; i++)
                {
                    paramsUsers[i] = this.DeluxeGridGroupUser.SelectedKeys[i];
                }
                WfGroupAdapter.Instance.DeleteGroupUsers(this.Data, paramsUsers);
                RefreshGrid();
            }
        }
        #endregion 

        #region GRID 
        
        public void RefreshGrid()
        {
            LastQueryRowCount = -1;
            if (this.Data != null)
            {
                whereCondition.Value = string.Format("GROUP_ID = {0}", MCS.Library.Data.Builder.TSqlBuilder.Instance.CheckQuotationMark(this.Data.GroupID, true));
            }
        }
        public void ExecQuery()
        {
            LastQueryRowCount = -1;
            this.DeluxeGridGroupUser.PageIndex = 0;
            if (this.Data != null)
            {
                whereCondition.Value = string.Format("GROUP_ID = {0}", MCS.Library.Data.Builder.TSqlBuilder.Instance.CheckQuotationMark(this.Data.GroupID, true));
            }
        }

        //protected void DeluxeGridGroupUser_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        WfGroupUser groupUser = (WfGroupUser)e.Row.DataItem;

        //        LinkButton delItem = (LinkButton)e.Row.FindControl("LinkBtnDel");
        //        delItem.CommandArgument = groupUser.User.ID;
        //        delItem.OnClientClick = "return window.confirm('确认要删除吗？');";
        //    }
        //}
        
        protected void ObjectDataSourceGroupUsers_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void ObjectDataSourceGroupUsers_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        #endregion

    }
}