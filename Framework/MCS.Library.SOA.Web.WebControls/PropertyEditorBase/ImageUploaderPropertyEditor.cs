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
