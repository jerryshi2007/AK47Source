
using System.Web.UI.Design;
namespace MCS.Web.WebControls
{
    public class DeluxeSearchClientDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {         
            var component = Component as DeluxeSearchClient;
            if (null != component)
            {
                return component.ID;
            }
            else
            {
                return GetEmptyDesignTimeHtml();
            }
        }
    }
}
