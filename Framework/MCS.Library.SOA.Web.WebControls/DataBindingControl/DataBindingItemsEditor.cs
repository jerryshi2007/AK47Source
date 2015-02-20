using System;
using System.Text;
using System.Web.UI.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace MCS.Web.WebControls
{
	internal class DataBindingItemsEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			/*
			PropertyDescriptor member = TypeDescriptor.GetProperties(context.Instance)["ItemBindings"];
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange((Component)context.Instance, new TransactedChangeCallback(this.InnerEditValues), null, null, member);
			*/

			DataBindingControl control = (DataBindingControl)context.Instance;
			DataBindingItemCollection bindings = (DataBindingItemCollection)control.ItemBindings;
			DataBindingItem binding = new DataBindingItem();

			binding.ControlID = "Shen Zheng";
			bindings.Add(binding);
			
			context.OnComponentChanged();
			return bindings;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		private bool InnerEditValues(object context)
		{
			bool changed = false;
			//DataBindingControl oControl = (DataBindingControl)Component;
			//IServiceProvider site = oControl.Site;
			IComponentChangeService changeService = null;
			try
			{
				/*
				DeluxeTreeItemsEditorForm oEditorForm = new DeluxeTreeItemsEditorForm(oControl);
				if (site != null)
				{
					IUIService service = (IUIService)site.GetService(typeof(IUIService));
					if (service != null)
					{
						if (service.ShowDialog(oEditorForm) == DialogResult.OK)
						{
							changed = true;
						}
					}
				}
				*/

			}
			finally
			{
				if (changed && changeService != null)
				{
					//changeService.OnComponentChanged(oControl, null, null, null);
				}
			}

			return changed;
		}
	}
}
