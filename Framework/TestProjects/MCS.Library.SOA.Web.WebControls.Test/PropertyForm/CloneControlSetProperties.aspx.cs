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
	public partial class CloneControlSetProperties : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("CloneControlProperties");

				this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);
			}

			//this.RegisterPropertyEditors();
		}

		private void RegisterPropertyEditors()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new OUUserInputPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
		}

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
	}
}