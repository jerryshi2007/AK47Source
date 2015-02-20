using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.ModelBinding;
using MCS.Library.WF.Contracts.Proxies;
using System.IO;
using System.Reflection;
 

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
	[ModelBinder(typeof(WFUpdateProcessParameterBinder))]
	public class WFUpdateProcessParameter : WFParameterWithResponseBase
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
       internal delegate PartialViewResult PartialViewHandler(string viewName); 
   
       private Controller _controller = null;
       private PartialViewHandler _viewHandler = null;


       /// <summary>
       /// 需要更新的客户端容器元素字典，key为容器元素ID，value为PartialView NAME
       /// </summary>
        public Dictionary<string, string> UpdateElements { set; get; } 

        private Dictionary<string, string> _updateElementsHtml = new Dictionary<string, string>();

        private Dictionary<string, object> _partialViewModel = new Dictionary<string, object>();
        private object _model = null;

        public void SetViewModel(object model)
        {
            _model = model;
        }
        /// <summary>
        /// 为PartialView视图设置model
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        public void SetPartialViewModel(string viewName,object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException("viewName");
            }
            if (!_partialViewModel.ContainsKey(viewName))
            {
                _partialViewModel.Add(viewName, model);
            }
            else
            {
                _partialViewModel[viewName] = model;
            }
        }
       
        internal void InitExecuteParams(Controller controller)
        {
            _controller = controller;
            PartialViewHandler handler = null;
            MethodInfo mth = controller.GetType().GetMethod("PartialView", BindingFlags.NonPublic|BindingFlags.Instance
               ,null,new Type[]{typeof(string)},null);
            handler = mth.CreateDelegate(typeof(PartialViewHandler),controller) as PartialViewHandler;
           
            _viewHandler = handler;
           
        }
        /// <summary>
        /// 获取需要更新的客户端容器元素字典，key为容器元素ID，value为PartialView 对应的HTML
        /// </summary>
        public Dictionary<string, string> UpdateElementsHtml
        {
            get
            {
                return _updateElementsHtml;
            }
        }
         
		protected override void InternalExecute(ResponseData response)
		{
            if (this.UpdateElements == null || this.UpdateElements.Count <= 0)
            {
                throw new ArgumentNullException("UpdateElements");
            }
            if (_viewHandler == null)
            {
                throw new ArgumentNullException("PartialViewHandler");
            }
            if (_controller == null)
            {
                throw new ArgumentNullException("Controller");
            }

            base.DefaultFillEmailCollector();
			response.ProcessInfo = WfClientProcessRuntimeServiceProxy.Instance.UpdateRuntimeParameters(this.ProcessId, this.RuntimeContext);

            GetUpdateElementsHtml();
		}

       
 

        private void GetUpdateElementsHtml()
        {

            HttpContextBase httpContext = _controller.HttpContext;           
            TextWriter tw = httpContext.Response.Output;
            System.Text.StringBuilder strB = new System.Text.StringBuilder();
            PartialViewResult pvr = null;
            _updateElementsHtml.Clear();

            httpContext.Request.ReloadWFContext();//刷新流程上下文

            foreach (var kvp in UpdateElements)
            {
                strB.Length = 0;

                pvr = _viewHandler(kvp.Value);
                using (httpContext.Response.Output = new System.IO.StringWriter(strB, tw.FormatProvider))
                {
                    if (_partialViewModel.ContainsKey(kvp.Value))//为Partview设置Model
                    {
                        _controller.ViewData.Model = _partialViewModel[kvp.Value];
                    }
                    else
                    {
                        _controller.ViewData.Model = _model;//页面Model
                    }
                    pvr.ExecuteResult(_controller.ControllerContext);
                }
                _updateElementsHtml.Add(kvp.Key, strB.ToString());
            }
            httpContext.Response.Output = tw;
            httpContext.Response.ClearContent(); 
        }

       
	}
}
