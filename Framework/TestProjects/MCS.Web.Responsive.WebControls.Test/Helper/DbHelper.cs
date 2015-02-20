using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Data;

namespace MCS.Web.Responsive.WebControls.Test
{
    public static class DbHelper
    {
        public static string ConnectionString = "defaultDatabase";

        public static DataSet RunSqlReturnDS(string strSql)
        {
            Database db = DatabaseFactory.Create(ConnectionString);

            return db.ExecuteDataSet(CommandType.Text, strSql);
        }

        public static object RunSqlReturnObject(string strSql)
        {
            Database db = DatabaseFactory.Create(ConnectionString);

            return db.ExecuteScalar(CommandType.Text, strSql);
        }
    }
}