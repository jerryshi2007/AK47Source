using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Responsive.WebControls
{
    [PropertyEditorDescription("ImageUploaderPropertyEditorForGrid", "MCS.Web.WebControls.ImageUploaderPropertyEditorForGrid")]
	public class ImageUploaderPropertyEditorForGrid : PropertyEditorBase
	{
		//private ImageUploaderDialog _imageUploaderDialog = new ImageUploaderDialog() { ID = "ImageUploaderPropertyEditorForGrid_ImageUploaderDialog",DialogTitle = "上传图片"};
		//private ImageUploader _imageUploader = new ImageUploader() { ID = "ImageUploaderPropertyEditorForGrid_ImageUploader" };

		protected internal override void OnPagePreRender(Page page)
		{
			if (page.Form != null)
			{
				var div = this.GetControlsContainerInPage(page);

				ImageUploaderDialog imageUploaderDialog = new ImageUploaderDialog() { ID = "ImageUploaderPropertyEditorForGrid_ImageUploaderDialog", DialogTitle = "上传图片" };
				ImageUploader imageUploader = new ImageUploader() { ID = "ImageUploaderPropertyEditorForGrid_ImageUploader" };

				div.Controls.Add(imageUploaderDialog);
				div.Controls.Add(imageUploader);
			}
		}
	}
}
