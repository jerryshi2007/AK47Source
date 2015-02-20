using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("OUUserInputPropertyEditor", "MCS.Web.WebControls.OUUserInputPropertyEditor")]
	public class OUUserInputPropertyEditor : PropertyEditorBase
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
			return "OuUserInputPropertyEditor_OuUserInputControl";
		}

		public override Control CreateDefalutControl()
		{
			return new OuUserInputControl() { ID = this.DefaultCloneControlName(), EnableViewState = false };
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
