using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class serverAddClientGrid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClientGrid cg = new ClientGrid()
                {
                    ID = "ClientGrid1",
                    OnCellCreatedEditor = "OnCellCreatedEditor",
                    Width = Unit.Point(820)
                };
                cg.Columns.Add(new ClientGridColumn()
                {
                    ItemStyle = "{width:'20px'}",
                    SelectColumn = true,
                    ShowSelectAll = true,
                });
                cg.Columns.Add(new ClientGridColumn()
                {
                    ItemStyle = "{width:'300px'}",
                    HeaderText = "员工编码",
                    DataField = "SelectedUserId",
                    DataType = DataType.String,
                });
                cg.Columns.Add(new ClientGridColumn()
                {
                    ItemStyle = "{width:'200px'}",
                    HeaderText = "员工姓名",
                    DataField = "SelectedUserDisplayName",
                    DataType = DataType.String
                });
                cg.Columns.Add(new ClientGridColumn()
                {
                    ItemStyle = "{width:'300px'}",
                    HeaderText = "人员标示",
                    DataField = "SelectedUserUserFullPath",
                    DataType = DataType.String
                });

                div_container.Controls.Add(cg);

                List<myDemoUser> list = new List<myDemoUser>();
                cg.InitialData = list;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ClientGrid cg = this.form1.FindControl("ClientGrid1") as ClientGrid;
            if (cg != null)
            {
                var data = cg.InitialData;
            }
        }
    }

    public class myDemoEntity
    {
        public string SelectedUserId { get; set; }
        public string SelectedUserDisplayName { get; set; }
        public string SelectedUserUserFullPath { get; set; }
    }
}