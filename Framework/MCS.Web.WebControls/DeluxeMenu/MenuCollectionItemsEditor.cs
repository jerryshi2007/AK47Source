using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace MCS.Web.WebControls
{
    internal class MenuCollectionItemsEditor : UITypeEditor

    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IDesignerHost service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
			DeluxeMenu component = (DeluxeMenu)context.Instance;
            ((MenuItemsDesigner)service.GetDesigner(component)).InvokeMenuItemCollectionEditor();
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

    }
}
