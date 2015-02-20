using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public partial class TestWithMaterialControl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gridTest.InitialData = new List<MaterialDataContainer>() { };

                MaterialAdapter.Instance.SaveDeltaMaterials(MaterialControl1.DeltaMaterials);
            }
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            var data = gridTest.InitialData;

            var deltaData = MaterialControl.GetCommonDeltaMaterials();
           
        }

        protected void btnSetReadOnly_Click(object sender, EventArgs e)
        {
            gridTest.ReadOnly = true;
        }

    }

    public class MaterialDataContainer
    {
        public IList<Material> Materials1
        {
            get;
            set;
        }

        public IList<Material> Materials2
        {
            get;
            set;
        }
    }
}