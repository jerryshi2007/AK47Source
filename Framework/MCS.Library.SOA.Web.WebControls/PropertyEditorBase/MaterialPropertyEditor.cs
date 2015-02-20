using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Caching;
using System.Reflection;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    public class MaterialPropertyEditor : PropertyEditorBase
    {
        public override bool IsCloneControlEditor
        {
            get
            {
                return true;
            }
        }

        public override string DefaultCloneControlName()
        {
            return "MaterialEditor_MaterialControl";
        }

        public override Control CreateDefalutControl()
        {
            return new MaterialControl() { ID = this.DefaultCloneControlName() };
        }

        protected internal override void OnPageInit(Page page)
        {
            //Callback时，提前创建模版控件，拦截请求
            if (page.IsCallback)
                CreateControls(page);
        }

        protected internal override void OnPagePreRender(Page page)
        {
            //除了CallBack，创建模版控件在LoadViewState之后，避免ViewState混乱
            if (!page.IsCallback)
                CreateControls(page);
        }
    }
}
