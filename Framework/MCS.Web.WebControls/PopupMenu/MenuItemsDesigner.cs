using System;
using System.IO;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using ChinaCustoms.Framework.DeluxeWorks.Web.Library;

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    internal class MenuItemsDesigner : DesignerBase
    {
        private DesignerVerbCollection _verbs;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("Éú³É²Ëµ¥...", new EventHandler(this.OnBuildMenu)) });
                }

                return _verbs;
            }
        }
        internal void InvokeMenuItemCollectionEditor()
        {
            this.EditMenuItems();
        }

        private void EditMenuItems()
        {
            PropertyDescriptor member = TypeDescriptor.GetProperties(base.Component)["Items"];
            ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.EditMenuItemsChangeCallback), null, null, member);
        }

        private bool EditMenuItemsChangeCallback(object context)
        {
            bool changed=false;
            PopupMenu oControl = (PopupMenu)Component;
            IServiceProvider site = oControl.Site;
            IComponentChangeService changeService = null;
            try
            {
                MenuItemsEditorForm oEditorForm = new MenuItemsEditorForm(oControl);
                if (oEditorForm.ShowDialog() == DialogResult.OK)
                {
                    changed = true;
                }
            }
            finally
            {
                if (changed && changeService != null)
                {
                    changeService.OnComponentChanged(oControl, null, null, null);
                }
            }
            return changed;
        }

        protected override string GetEmptyDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Right-click and select Build Menu for a quick start.");
        }

        private void OnBuildMenu(object sender, EventArgs e)
        {
            MenuItemsEditor oEditor = new MenuItemsEditor();
            oEditor.EditComponent(Component);
        }
    }
}


