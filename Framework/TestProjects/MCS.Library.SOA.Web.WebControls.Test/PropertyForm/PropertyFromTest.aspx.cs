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
	public partial class PropertyFromTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
				layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

				this.propertyForm.Layouts.InitFromLayoutSectionCollection(layouts);

				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("OUUser");

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
			//PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());

			//PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new OUUserInputPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new RoleGraphPropertyEditor());
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			string propertiescount = this.propertyForm.Properties.Count.ToString();
			string lacount = this.propertyForm.Layouts.Count.ToString();
		}

		protected override void OnPreRender(EventArgs e)
		{
			string propertiescount = this.propertyForm.Properties.Count.ToString();
			string lacount = this.propertyForm.Layouts.Count.ToString();
			base.OnPreRender(e);
		}

		protected override void OnPreRenderComplete(EventArgs e)
		{
			string propertiescount = this.propertyForm.Properties.Count.ToString();
			string lacount = this.propertyForm.Layouts.Count.ToString();
			base.OnPreRenderComplete(e);
		}
	}
}