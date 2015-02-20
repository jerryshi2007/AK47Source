using System.IO;
using System.Web.UI;
using System.Web.UI.Design;

namespace MCS.Web.WebControls
{
    public class PopupTipDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            var ctrl = this.Component as PopupTip;
            var writer = new StringWriter();
            var html = new HtmlTextWriter(writer);
            if (ctrl != null)
            {
                writer.WriteLine(ctrl.ID);      
            }
            return writer.ToString();
        }
    }
}
