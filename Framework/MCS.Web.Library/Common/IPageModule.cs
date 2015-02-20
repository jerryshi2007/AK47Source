using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Library
{
    /// <summary>
    /// IPageModule
    /// </summary>
    public interface IPageModule
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="page"></param>
        void Init(Page page);

        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();
    }

    /// <summary>
    /// BasePageModule
    /// </summary>
    public class BasePageModule : IPageModule
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="page"></param>
        protected virtual void Init(Page page)
        {
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose()
        {
        }

        void IPageModule.Init(Page page)
        {
            this.Init(page);
        }

        void IPageModule.Dispose()
        {
            this.Dispose();
        }
    }
}
