using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using System.Web.UI.HtmlControls;
using MCS.Library.Principal;

namespace MCS.OA.CommonPages.WFPost
{
    public partial class WFPostManager : System.Web.UI.Page
    {
        public int LastQueryRowCount
        {
            get { return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1); }
            set { WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack)
            {
                ExecQuery();
            }
        }

        public void ExecQuery()
        {
            LastQueryRowCount = -1;
            this.DeluxeGridPost.PageIndex = 0;
        }

        protected void DeluxeGridPost_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                WfPost post = (WfPost)e.Row.DataItem;

                HtmlAnchor editItem = (HtmlAnchor)e.Row.FindControl("LinkBtnEdit");
                editItem.Attributes.Add("onclick", "onCreateOrUpdateClick('" + post.PostID + "')");

                LinkButton delItem = (LinkButton)e.Row.FindControl("LinkBtnDel");
                delItem.CommandArgument = post.PostID;
                delItem.OnClientClick = "return window.confirm('确认要删除吗？');";

                #region 格式化数据
                e.Row.Cells[3].Text = post.CreateTime.ToString("yyyy-MM-dd");
                #endregion
            }
        }

        protected void DeluxeGridPost_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //删除岗位
            if (e.CommandName == "DeletePost")
            {

                WfPostCollection postCollection = WfPostAdapter.Instance.Load(builder => builder.AppendItem("POST_ID", e.CommandArgument.ToString()));
                if (postCollection.Count > 0)
                {
                    WfPostAdapter.Instance.Delete(postCollection[0]);
                    RefreshButton_Click(sender, new EventArgs());
                }
            }
        }

        protected void DeluxeGridPost_ExportData(object sender, EventArgs e)
        {
            ExecQuery();
        }

        protected void ObjectDataSourcePost_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void ObjectDataSourcePost_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            LastQueryRowCount = -1;
            this.DeluxeGridPost.DataBind();
        }
    }
}