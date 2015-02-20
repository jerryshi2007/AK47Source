using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using System.Data;

namespace MCS.OA.CommonPages
{
    public partial class ZyTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            MaterialContentCollection materials = MaterialContentAdapter.Instance.Load(p => p.AppendItem("RELATIVE_ID", "100"));

            Upload(materials[0]);
            materials[0].RelativeID = "101";
            Upload(materials[0]);
            materials[0].RelativeID = "102";
            Upload(materials[0]);
            materials[0].RelativeID = "103";
            Upload(materials[0]);
            materials[0].RelativeID = "104";
            Upload(materials[0]);
            materials[0].RelativeID = "105";
            Upload(materials[0]);
        }

        private void Upload(MaterialContent data)
        {
            data.ContentID = UuidHelper.NewUuidString();
            string sql = ORMapping.GetInsertSql(data, TSqlBuilder.Instance);
            DbHelper.RunSql(db => db.ExecuteNonQuery(CommandType.Text, sql), "UAT");
        }
    }
}