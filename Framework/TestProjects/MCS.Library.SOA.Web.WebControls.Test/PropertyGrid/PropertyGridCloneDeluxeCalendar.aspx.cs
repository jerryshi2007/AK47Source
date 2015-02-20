using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyGrid
{
	public partial class PropertyGridCloneDeluxeCalendar : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("DateTimeFieldProperties");

				this.propertyGrid.Properties.InitFromPropertyDefineCollection(propeties);
			}

			this.PropertyEditorRegister();
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
		}
	}
}