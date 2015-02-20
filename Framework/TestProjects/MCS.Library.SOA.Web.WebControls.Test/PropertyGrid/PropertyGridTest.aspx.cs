using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Web.Library;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyGrid
{
	public partial class PropertyGridTest : System.Web.UI.Page
	{
		protected override void OnPreInit(EventArgs e)
		{
			PropertyEditorRegister();

			base.OnPreInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// InitEnumValues();
			if (this.Page.IsPostBack == false)
			{

				PropertyDefineCollection propeties = new PropertyDefineCollection();
				//propeties.LoadPropertiesFromConfiguration("ValidateProperties");
				propeties.LoadPropertiesFromConfiguration("IntegerFieldProperties");

				propertyGrid.Properties.InitFromPropertyDefineCollection(propeties);


				//PropertyLayoutSectionCollection layouts = new PropertyLayoutSectionCollection();
				//layouts.LoadLayoutSectionFromConfiguration("DefalutLayout");

				//this.propertyForm.Layouts.InitFromLayoutSectionCollection(layouts);


				//this.propertyForm.Properties.InitFromPropertyDefineCollection(propeties);

			}
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());

			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditorForGrid());
			PropertyEditorHelper.RegisterEditor(new OUUserInputPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new RoleGraphPropertyEditor());
		}

		protected void bT_Click(object sender, EventArgs e)
		{
            var imgProStr = propertyGrid.Properties.GetValue("Image1", "");
            if (imgProStr != "")
            {
                ImageProperty imgPro = JSONSerializerExecute.Deserialize<ImageProperty>(imgProStr);
                if (imgPro.Changed)
                {
                    ImagePropertyAdapter.Instance.UpdateWithContent(imgPro);
                    propertyGrid.Properties.SetValue("Image1", JSONSerializerExecute.Serialize(imgPro));
                }
            }

			imgProStr = propertyGrid.Properties.GetValue("Image2", "");
			if (imgProStr != "")
			{
				ImageProperty imgPro = JSONSerializerExecute.Deserialize<ImageProperty>(imgProStr);
				if (imgPro.Changed)
				{
					ImagePropertyAdapter.Instance.UpdateWithContent(imgPro);
					propertyGrid.Properties.SetValue("Image2", JSONSerializerExecute.Serialize(imgPro));
				}
			}
		}


		//private void InitEnumValues()
		//{
		//    Dictionary<string, Dictionary<string, string>> enumTypes = new Dictionary<string, Dictionary<string, string>>();
		//    Dictionary<string, string> items = new Dictionary<string, string>();
		//    items.Add("AA", "BB");
		//    items.Add("BB", "CC");
		//    items.Add("CC", "DD");
		//    enumTypes.Add("EnumValue", items);

		//    this.enumTypeStore.Value = JSONSerializerExecute.Serialize(enumTypes);
		//}
	}
}