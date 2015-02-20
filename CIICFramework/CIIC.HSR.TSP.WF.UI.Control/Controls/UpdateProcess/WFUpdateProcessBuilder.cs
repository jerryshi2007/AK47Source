using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.UpdateProcess
{
	public class WFUpdateProcessBuilder : WFButtonControlBuilderlBase<WFUpdateProcess, WFUpdateProcessBuilder>
	{
		public WFUpdateProcessBuilder(HtmlHelper htmlHelper)
			: base(new WFUpdateProcess(htmlHelper.ViewContext, htmlHelper.ViewData), htmlHelper)
		{
		}

	    /// <summary>
	    /// 添加刷新区域
	    /// </summary>
	    /// <param name="elementID">容器元素ID</param>
        /// <param name="partialvViewName">partialvViewName</param>
	    /// <returns></returns>
        public WFUpdateProcessBuilder AddUpdateElement(string elementID, string partialvViewName)
		{
            if(string.IsNullOrEmpty(elementID))
            {
                throw new ArgumentNullException("elementID");
            }
            if (string.IsNullOrEmpty(partialvViewName))
            {
                throw new ArgumentNullException("partialvViewName");
            }
            this.Component.UpdateElements.Add(elementID, partialvViewName);
			return this;
		}
	}
}
