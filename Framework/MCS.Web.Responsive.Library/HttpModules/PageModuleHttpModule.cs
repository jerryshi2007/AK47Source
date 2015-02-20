using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
    /// <summary>
    /// ¼ÓÔØPageModuleµÄHttpModule
    /// </summary>
    public class PageModuleHttpModule : IHttpModule
    {
        private Dictionary<string, IPageModule> _PageModules = null;

        /// <summary>
        /// Init HttpModule
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += new EventHandler(PreRequestHandlerExecuteHandler);
        }

        /// <summary>
        /// Dispose HttpModule
        /// </summary>
        protected virtual void Dispose()
        {
            if (_PageModules != null)
            {
                foreach (IPageModule module in _PageModules.Values)
                {
                    module.Dispose();
                }
            }
        }

        private void PreRequestHandlerExecuteHandler(object sender, EventArgs e)
        {
            Page page = HttpContext.Current.CurrentHandler as Page;

            if (page != null)
            {
                _PageModules = ConfigSectionFactory.GetPageModulesSection().Create();
                foreach (IPageModule module in _PageModules.Values)
                {
                    module.Init(page);
                }
            }
        }

        void IHttpModule.Init(HttpApplication context)
        {
            this.Init(context);
        }

        void IHttpModule.Dispose()
        {
            this.Dispose();
        }
    }
}
