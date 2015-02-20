using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Web.Library;

namespace WeChatManage.ModalDialogs
{
    public partial class CheckGroupMembers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var groupID = WebUtility.GetRequestQueryValue("groupID", "");

                if (groupID.IsNotEmpty())
                {
                    WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
                    builder.AppendItem("g.GroupID", groupID);
                    gridDataSource.Condition = builder.ToSqlString(TSqlBuilder.Instance);
                    gridMain.DataBind();
                }
            }
        }
    }
}