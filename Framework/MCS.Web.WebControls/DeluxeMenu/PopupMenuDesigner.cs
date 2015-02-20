using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;

namespace MCS.Web.WebControls
{
    class PopupMenuDesigner : ControlDesigner
    {
        protected override string GetEmptyDesignTimeHtml()
        {
            return "<span>没有条目</span>";
        }

        public override string GetDesignTimeHtml()
        {
            try
            {
				DeluxeMenu m = (DeluxeMenu)this.ViewControl;

                if (m.Items.Count == 0)
                    return this.GetEmptyDesignTimeHtml();
                else
                    return this.GetMenuDesignHTML(m);
            }
            catch (Exception ex)
            {
                return GetErrorDesignTimeHtml(ex);
            }
            //return string.Format("<span>{0}</span>", menu.ID);
        }       

        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            return base.GetErrorDesignTimeHtml(e);
        }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();
                verbs.Add(new DesignerVerb("添加条目", new EventHandler(this.OnAddItem)));
                return verbs;
            }
        }

        private void OnAddItem(object sender, EventArgs args)
        {
            this.EditMenuItems();
        }

        private void EditMenuItems()
        {
            //PropertyDescriptor member = TypeDescriptor.GetProperties(base.Component)["Items"];
            //ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.EditMenuItemsChangeCallback), null, "Edit Items", member);
        }

        private bool EditMenuItemsChangeCallback(object context)
        {
           // IServiceProvider serviceProvider = this._menu.Site;
            return true;
            //MenuItemCollectionEditorDialog form = new MenuItemCollectionEditorDialog(this._menu, this);
            //return (UIServiceHelper.ShowDialog(serviceProvider, form) == DialogResult.OK);
        }

		private string GetMenuDesignHTML(DeluxeMenu menu)
        {
            StringBuilder strB = new StringBuilder();
            strB.Append("<Table>");
            strB.Append(this.GetMenuItemsDesignHTML(menu.Items, 0));
            strB.Append("</Table>");

            return strB.ToString();
        }

        private string GetMenuItemsDesignHTML(IList<MCS.Web.WebControls.MenuItem> items, int level)
        {
            StringBuilder strB = new StringBuilder();
            foreach (MCS.Web.WebControls.MenuItem item in items)
            {
                strB.Append("<TR>");
                strB.AppendFormat("<TD style='text-indent:{0}px'>{1}</TD>", level * 10, item.Text);
                strB.Append("</TR>");
                strB.Append(this.GetMenuItemsDesignHTML(item.ChildItems, level + 1));
            }

            return strB.ToString();
        }
    }
}
