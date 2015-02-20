using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages
{
    public partial class ZyTestMaterial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string resourceID = "100";
                MaterialControl2.DefaultResourceID = resourceID;
            }
        }

        protected void btnSaveMaterialControl2_Click(object sender, EventArgs e)
		{
            MaterialAdapter.Instance.SaveDeltaMaterials(MaterialControl2.DeltaMaterials);
        }

        
    }
}