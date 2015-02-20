using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 角色选择的属性编辑器
    /// </summary>
    public class RoleGraphPropertyEditor : PropertyEditorBase
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
            return "RoleGraphPropertyEditor_RoleGraphControl";
        }

        public override Control CreateDefalutControl()
        {
            return new RoleGraphControl() { ID = this.DefaultCloneControlName(), EnableViewState = false, DialogTitle = RoleGraphControlParams.DefaultDialogTitle };
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
            if (page.IsCallback == false)
                CreateControls(page);
        }
    }
}
