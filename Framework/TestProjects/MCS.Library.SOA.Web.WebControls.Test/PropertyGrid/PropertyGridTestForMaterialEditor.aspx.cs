using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyGrid
{
    public partial class PropertyGridTestForMaterialEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PropertyDefineCollection propeties = new PropertyDefineCollection();
                propeties.LoadPropertiesFromConfiguration("MaterialEditorTest");

                this.PropertyGrid.Properties.InitFromPropertyDefineCollection(propeties);

                RegisterPropertyEditors();
            }

        }

        private void RegisterPropertyEditors()
        {
            PropertyEditorHelper.RegisterEditor(new MaterialPropertyEditor());
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            var properties = this.PropertyGrid.Properties;
            //this.PropertyGrid.ReadOnly = true;
        }
    }
}