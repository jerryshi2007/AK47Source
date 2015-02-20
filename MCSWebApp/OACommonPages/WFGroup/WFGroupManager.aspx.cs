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

namespace MCS.OA.CommonPages.WFGroup
{
    public partial class WFGroupManager : System.Web.UI.Page
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
            this.DeluxeGridGroup.PageIndex = 0;
        }

        protected void DeluxeGridGroup_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                WfGroup group = (WfGroup)e.Row.DataItem;

                HtmlAnchor editItem = (HtmlAnchor)e.Row.FindControl("LinkBtnEdit");
                editItem.Attributes.Add("onclick", "onCreateOrUpdateClick('" + group.GroupID + "')");

                LinkButton delItem = (LinkButton)e.Row.FindControl("LinkBtnDel");
                delItem.CommandArgument = group.GroupID;
                delItem.OnClientClick = "return window.confirm('确认要删除吗？');";

                #region 格式化数据 
                e.Row.Cells[4].Text = group.CreateTime.ToString("yyyy-MM-dd");
                #endregion
            }
        }

        protected void DeluxeGridGroup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //删除群组;
            if (e.CommandName == "DeleteGroup")
            {
                WfGroupCollection groupCollection = WfGroupAdapter.Instance.Load(builder => builder.AppendItem("GROUP_ID", e.CommandArgument.ToString()));
                if (groupCollection.Count > 0)
                {
                    WfGroupAdapter.Instance.Delete(groupCollection[0]);
                    RefreshButton_Click(sender, new EventArgs());
                }
            }
        }

        protected void DeluxeGridGroup_ExportData(object sender, EventArgs e)
        {
            ExecQuery();
        }

        protected void ObjectDataSourceGroup_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void ObjectDataSourceGroup_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            LastQueryRowCount = -1;
            this.DeluxeGridGroup.DataBind();
        }
    }
}