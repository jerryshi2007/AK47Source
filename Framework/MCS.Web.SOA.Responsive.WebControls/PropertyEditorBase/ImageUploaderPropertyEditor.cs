using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("ImageUploaderPropertyEditor", "MCS.Web.WebControls.ImageUploaderPropertyEditor")]
	public class ImageUploaderPropertyEditor : PropertyEditorBase
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
			return "ImageUploaderPropertyEditor_ImageUploader";
		}

		public override Control CreateDefalutControl()
		{
			return new ImageUploader() { ID = this.DefaultCloneControlName() };
		}

		protected internal override void OnPageInit(Page page)
		{
			//if (page.IsCallback)
			//    CreateControls(page);
		}

		protected internal override void OnPagePreRender(Page page)
		{
			//if (!page.IsCallback)
			CreateControls(page);
		}
	}
}
