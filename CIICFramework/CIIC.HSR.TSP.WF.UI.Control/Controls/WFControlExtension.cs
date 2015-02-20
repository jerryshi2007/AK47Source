using CIIC.HSR.TSP.WF.UI.Control.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control
{
    public static class WFControlExtension
    {
        public static WFWidgetFactory<TModel> HSRUIWF<TModel>(this HtmlHelper<TModel> helper)
        {
            return new WFWidgetFactory<TModel>(helper);
        }
    }
}
