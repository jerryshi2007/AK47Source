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
	public partial class PropertyEditorBaseTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				#region "propertyGrid"
				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("DateTimeFieldProperties");

				this.propertyGrid.Properties.InitFromPropertyDefineCollection(propeties);
				#endregion

				#region "propertyForm"
				PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
				layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

				this.propertyForm.Layouts.InitFromLayoutSectionCollection(layouts);

				PropertyDefineCollection formPropeties = new PropertyDefineCollection();
				formPropeties.LoadPropertiesFromConfiguration("IntegerFieldProperties");

				this.propertyForm.Properties.InitFromPropertyDefineCollection(formPropeties);
				#endregion
			}

			PropertyEditorRegister();
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new OUUserInputPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditorForGrid());
		}
	}
}