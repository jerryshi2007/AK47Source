using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.Library
{
    internal class DataBoundControlInternal : DataBoundControl
    {
        private IEnumerable _DataSourceResult;

        public DataBoundControlInternal()
            : base()
        {
            ConfirmInitState();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.RequiresDataBinding = false;
        }

        public IEnumerable DataSourceResult
        {
            get
            {
                this.EnsureDataBound();                    
                return _DataSourceResult;
            }
        }

        public void SetRequiresDataBinding(bool requiresDataBinding)
        {
            RequiresDataBinding = requiresDataBinding;
        }

        protected override void PerformSelect()
        {
            this.GetData().Select(DataSourceSelectArguments.Empty, new DataSourceViewSelectCallback(SetDataSourceResult));
        }

        private void SetDataSourceResult(IEnumerable data)
        {
            _DataSourceResult = data;
        }

        protected override void EnsureDataBound()
        {
            if (RequiresDataBinding)
            {
                this.DataBind();
                this.RequiresDataBinding = false;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {            
        }
    }
}
