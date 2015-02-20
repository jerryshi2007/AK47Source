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
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using MCS.Web.Library;
using System.Security.Permissions;
using System.Windows.Forms.Design;

namespace MCS.Web.WebControls
{
    [SupportsPreviewControl(true), SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal class DeluxeTreeItemsDesigner : DesignerBase
    {
        private DesignerVerbCollection verbs;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (this.verbs == null)
                {
                    this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("Éú³ÉÊ÷...", new EventHandler(this.OnBuildMenu)) });
                }

                return this.verbs;
            }
        }

        internal void InvokeMenuItemCollectionEditor()
        {
            this.EditMenuItems();
        }

        private void EditMenuItems()
        {
            PropertyDescriptor member = TypeDescriptor.GetProperties(base.Component)["Items"];
            System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.EditMenuItemsChangeCallback), null, null, member);
        }

        private bool EditMenuItemsChangeCallback(object context)
        {
            bool changed = false;
            DeluxeTree oControl = (DeluxeTree)Component;
            IServiceProvider site = oControl.Site;
            IComponentChangeService changeService = null;
            try
            {
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
            DeluxeTreeItemsEditor oEditor = new DeluxeTreeItemsEditor();
            oEditor.EditComponent(Component);
        }

        private string GetControlHtml(System.Web.UI.Control ctrl)
        {
            StringBuilder strB = new StringBuilder(1024);

            StringWriter sw = new StringWriter(strB);

            try
            {
                HtmlTextWriter writer = new HtmlTextWriter(sw);
                ctrl.RenderControl(writer);

                return strB.ToString();
            }
            finally
            {
                sw.Close();
            }
        }
    }
}


