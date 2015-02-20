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
	public partial class EmptyCategoryTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			PropertyDefineCollection propeties = new PropertyDefineCollection();
			propeties.LoadPropertiesFromConfiguration("EmptyCategoryProperties");

			this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);

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
			PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());
		}

	}
}