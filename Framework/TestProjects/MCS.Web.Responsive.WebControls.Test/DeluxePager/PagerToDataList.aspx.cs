using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data;

namespace MCS.Web.Responsive.WebControls.Test.DeluxePager
{
    public partial class PagerToDataList : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            sqlDataSource.ConnectionString = DbConnectionManager.GetConnectionString("defaultDatabase"); ;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            e.Command.Connection.Open();

            try
            {
                e.Command.Parameters.Clear();
                e.Command.CommandText = "select count(*) from Orders";

                object obj = e.Command.ExecuteScalar();
                if (obj != null)
                    pager.RecordCount = (int)obj;
            }
            finally
            {
                e.Command.Connection.Close();
            }
        }
    }
}