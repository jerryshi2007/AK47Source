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
	public partial class PasswordConfirmationEditorTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				PropertyDefineCollection propeties = new PropertyDefineCollection();
				propeties.LoadPropertiesFromConfiguration("PasswordConfirmationProperties");

				this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);
			}

			this.RegisterPropertyEditors();
		}

		private void RegisterPropertyEditors()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());

			PropertyEditorHelper.RegisterEditor(new PasswordConfirmationEditor());
		}

		protected void UserPasswordPersister_Click(object sender, EventArgs e)
		{
			this.propertyForm.Properties.Write();
		}
	}
}