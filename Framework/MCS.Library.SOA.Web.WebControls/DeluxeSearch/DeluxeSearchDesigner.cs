
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    public class DeluxeSearchDesigner : ControlDesigner
    {   

        public override string GetDesignTimeHtml()
        {
            var style = new TableStyle();
            var table = new Table();
            var component = Component as DeluxeSearch;
            var writer = new StringWriter();
            var html = new HtmlTextWriter(writer);
            if (null != component)
            {
                if (component.Categories.Count > 0)
                {
                    foreach (var item in component.Categories)
                    {
                        var cell = new TableCell();
                        var row = new TableRow();
                        cell.Text = string.Format("{0}:{1}", item.DataTextField, item.DataSourceID);
                        row.Cells.Add(cell);
                        table.Rows.Add(row);
                    }
                }
                else
                {
                    var cell = new TableCell();
                    var row = new TableRow();
                    cell.Text = component.ID;
                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                }
                table.ApplyStyle(style);              
                table.RenderControl(html);
            }
            return writer.ToString();
        }
    }
}
