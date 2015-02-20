using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using System.Web.UI.HtmlControls;

namespace MCS.OA.CommonPages.UserMapping
{
    public partial class UserMappingManager : System.Web.UI.Page
    {
        public int LastQueryRowCount
        {
            get { return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1); }
            set { WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ExecQuery();
        }

        public void ExecQuery()
        {
            LastQueryRowCount = -1;
            this.DeluxeGridExtUserMapping.PageIndex = 0;
        }

        protected void DeluxeGridExtUserMapping_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ExtUserMapping extUser = (ExtUserMapping)e.Row.DataItem;

                HtmlAnchor editItem = (HtmlAnchor)e.Row.FindControl("LinkBtnEdit");
                editItem.Attributes.Add("onclick", "onCreateOrUpdateClick('" + extUser.UserID + "','"+ extUser.MappingUserID +"')");

                LinkButton delItem = (LinkButton)e.Row.FindControl("LinkBtnDel");
                delItem.CommandArgument = extUser.UserID;
                delItem.OnClientClick = "return window.confirm('确认要解除映射关系吗？');";

                e.Row.Cells[2].Text = ParseEnumName(extUser);
                e.Row.Cells[3].Text = ParseStatusName(extUser);

                #region 格式化数据 
                e.Row.Cells[4].Text = extUser.CreateTime.ToString("yyyy-MM-dd");
                #endregion
            }
        }

        private string ParseStatusName(ExtUserMapping userMapping)
        {
            if (userMapping != null)
            {
                return userMapping.Status == 0 ? "停用" : "启用";
            }
            return string.Empty;
        }
        private string ParseEnumName(ExtUserMapping userMapping)
        {
            if (userMapping != null)
            {
                switch (userMapping.UserType)
                {
                    case ExtUserTypes.Custom:
                        return "客户";
                    case ExtUserTypes.GovernmentSociety:
                        return "政务及社会";
                    case ExtUserTypes.Investor:
                        return "投资者";
                    case ExtUserTypes.Partner:
                        return "合作伙伴";
                    case ExtUserTypes.Public:
                        return "公众";
                    default:
                        return "";
                }
            }
            return string.Empty;
        }
        protected void DeluxeGridExtUserMapping_ExportData(object sender, EventArgs e)
        {
            ExecQuery();
        }

        protected void DeluxeGridExtUserMapping_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //删除;
            if (e.CommandName == "DeleteMapping")
            {
               ExtUserMappingAdapter.Instance.DeleteMappingUser(e.CommandArgument.ToString());
               RefreshButton_Click(sender, new EventArgs());
            }
        }

        protected void ObjectDataSourceUserMapping_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            LastQueryRowCount = (int)e.OutputParameters["totalCount"];
        }

        protected void ObjectDataSourceUserMapping_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["totalCount"] = LastQueryRowCount;
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            LastQueryRowCount = -1;
            this.DeluxeGridExtUserMapping.DataBind();
        }
        
    }
}