using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;

namespace WeChatManage
{
    public partial class ConditionGroups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void gridMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var groupID = DataBinder.GetPropertyValue(e.Row.DataItem, "GroupID").ToString();
                var gm = GroupAndMemberAdapter.Instance.LoadByGroupID(groupID);

                var cnt = gm.Count;

                e.Row.Cells[2].Text = cnt.ToString();
                
            }
        }
    }
}