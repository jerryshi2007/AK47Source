using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyForm
{
    public partial class PropertyFormTestForClientGridEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
                layouts.LoadLayoutSectionFromConfiguration("GridLayout");

                this.propertyForm.Layouts.InitFromLayoutSectionCollection(layouts);

                PropertyDefineCollection propeties = new PropertyDefineCollection();
                propeties.LoadPropertiesFromConfiguration("ClientGridEditorTest");

                this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);

                //RegisterPropertyEditors();
                //已配置
            }

        }

        private void RegisterPropertyEditors()
        {
            PropertyEditorHelper.RegisterEditor(new ClientGridPropertyEditor());
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            var properties = this.propertyForm.Properties;
            //this.propertyForm.ReadOnly = true;
        }
    }
}