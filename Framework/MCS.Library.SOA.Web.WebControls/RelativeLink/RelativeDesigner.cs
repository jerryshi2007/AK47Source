using System.IO;
using System.Web.UI;
using System.Web.UI.Design;

namespace MCS.Web.WebControls
{
    public class RelativeDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            var ctrl = Component as RelativeLink;
            var writer = new StringWriter();
            new HtmlTextWriter(writer);
            if (ctrl != null)
            {
                writer.WriteLine(ctrl.ID);
            }

            return writer.ToString();
        }
    }
}
