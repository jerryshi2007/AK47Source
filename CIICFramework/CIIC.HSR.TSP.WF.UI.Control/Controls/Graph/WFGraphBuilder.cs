using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using CIIC.HSR.TSP.WebComponents;
using System.Web.Mvc; 
using MCS.Web.Library.Script;
 

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Graph
{
    public class WFGraphBuilder : WidgetBuilderBase<WFGraph, WFGraphBuilder>
    {
        public WFGraphBuilder(HtmlHelper htmlHelper)
            : base(new WFGraph(htmlHelper.ViewContext,htmlHelper.ViewData), htmlHelper)
        {    
        }

        /// <summary>
        /// 激活流程节点上用户为空时候，是否显示默认值
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public WFGraphBuilder EnableDefaultUserName(bool enable)
        {             
            this.Component.Operations.EnableDefaultUserName = enable;         
            return this;
        }

        /// <summary>
        /// 激活流程节点上用户为空时候，默认值显示的信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public WFGraphBuilder DefaultUserName(string username)
        {
            this.Component.Operations.DefaultUserName = username;
            return this;
        }

    }
}
