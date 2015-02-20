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

namespace MCS.OA.CommonPages.WFPost
{
    public partial class WFPostEditor : System.Web.UI.Page
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

        public WfPost Data
        {
            get { return (WfPost)ViewState["Data"]; }
            set { ViewState["Data"] = value; }
        }
        public int LastQueryRowCount
        {
            get { return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1); }
            set { WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value); }
        }

        #region Request Processors

        [ControllerMethod(true)]
        protected void NewPostRequest()
        {
            whereCondition.Value = "1=2";
            Data = new WfPost();
            Data.PostID = string.Empty;
        }

        [ControllerMethod]
        protected void EditPostRequest(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                NewPostRequest();
            }
            else
            {
                WfPostCollection posts = WfPostAdapter.Instance.Load(builder => builder.AppendItem("POST_ID", postId));
                ExceptionHelper.FalseThrow(posts.Count > 0, "没有找到对应的岗位定义");
                Data = posts[0];
                ExecQuery();
            }
        }

        #endregion

        #region Update Or Insert POST

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            this.bindingControl.CollectData(); 
            if (string.IsNullOrEmpty(this.Data.PostName))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "alert('请填写岗位名称！');", true);
                return;
            }
            DeluxeIdentity identity = HttpContext.Current.User.Identity as DeluxeIdentity;
            ExceptionHelper.TrueThrow(identity == null, "无法获取登陆用户");
            bool newGroup = string.IsNullOrEmpty(Data.PostID);
            try
            {
                this.bindingControl.CollectData();
                if (newGroup)
                {
                    Data.PostID = Guid.NewGuid().ToString();
                    Data.CreateTime = System.DateTime.Now;
                    Data.Creator = identity.User;
                }
                WfPostAdapter.Instance.Update(Data);
            }
            catch (SqlException sqlEx)
            {
                if (newGroup)
                {
                    Data.PostID = string.Empty;
                }
                WebUtility.ShowClientError(sqlEx.Message, sqlEx.StackTrace, "错误");
            }
            catch (Exception ex)
            {
                WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
            }
        }

        #endregion

        #region DELETE USERS

        //protected void DeluxeGridPostUser_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    //删除岗位用户
        //    if (e.CommandName == "DeletePostUser")
        //    {
        //        if (this.data != null && !string.IsNullOrEmpty(this.data.PostID))
        //        {
        //            WFPostAdapter.Instance.DeletePostUsers(this.data, e.CommandArgument.ToString());
        //        }
        //    }
        //}

        protected void BtnDeleteUsers_Click(object sender, EventArgs e)
        {
            if (this.Data != null && !string.IsNullOrEmpty(this.Data.PostID) 
                &&this.DeluxeGridPostUser.SelectedKeys.Count>0)
            {
                string[] paramsUsers = new string[this.DeluxeGridPostUser.SelectedKeys.Count];
                for (int i = 0; i < this.DeluxeGridPostUser.SelectedKeys.Count; i++)
                {
                    paramsUsers[i] = this.DeluxeGridPostUser.SelectedKeys[i];
                }
                WfPostAdapter.Instance.DeletePostUsers(this.Data, paramsUsers);
                RefreshGrid();
            }
        }

        #endregion

        #region ADD USERS

        protected void BtnAddUsers_Click(object sender, EventArgs e)
        {
            #region validate 
            if (this.Data != null && string.IsNullOrEmpty(this.Data.PostID))
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "alert('请先保存岗位！');", true);
                return;
            }
            if (this.postUserInput.SelectedOuUserData.Count == 0)
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "alert('请先选择要添加的用户！');", true);
                return;
            }
            #endregion

            this.bindingControl.CollectData();
            IList<IUser> users = new List<IUser>();
            for (int i = 0; i < this.postUserInput.SelectedOuUserData.Count; i++)
            {
                users.Add(this.postUserInput.SelectedOuUserData[i] as IUser);
            }
            WfPostAdapter.Instance.AddPostUsers(this.Data, users);
            RefreshGrid();

        }

        #endregion

        #region GRID 

        public void ExecQuery()
        {
            LastQueryRowCount = -1;
            this.DeluxeGridPostUser.PageIndex = 0;
            if (this.Data != null)
            {
                whereCondition.Value = string.Format("POST_ID = {0}", MCS.Library.Data.Builder.TSqlBuilder.Instance.CheckQuotationMark(this.Data.PostID, true));
            }
        }

        public void RefreshGrid()
        {
            LastQueryRowCount = -1;
            if (this.Data != null)
            {
                whereCondition.Value = string.Format("POST_ID = {0}", MCS.Library.Data.Builder.TSqlBuilder.Instance.CheckQuotationMark(this.Data.PostID, true));
            }
        }

        //protected void DeluxeGridPostUser_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        WfPostUser post = (WfPostUser)e.Row.DataItem;

        //        LinkButton delItem = (LinkButton)e.Row.FindControl("LinkBtnDel");
        //        delItem.CommandArgument = post.User.ID;
        //        delItem.OnClientClick = "return window.confirm('确认要删除吗？');";
        //    }
        //}

        protected void ObjectDataSourcePostUsers_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void ObjectDataSourcePostUsers_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        #endregion

    }
}