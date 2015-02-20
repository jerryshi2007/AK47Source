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
	public partial class PropertyFormValidatorTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
				layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

				this.propertyForm.Layouts.InitFromLayoutSectionCollection(layouts);

				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("ValidateProperties");

				this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);
			}

			RegisterPropertyEditors();
		}

		private void RegisterPropertyEditors()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			/*
			foreach (PropertyValue pv in this.propertyForm.Properties)
			{

			} */
		}
	}
}