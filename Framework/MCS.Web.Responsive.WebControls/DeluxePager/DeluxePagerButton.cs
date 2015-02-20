using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Responsive.WebControls
{
    internal class DeluxePagerButton : LinkButton
    {
        private IPostBackContainer container;

        public DeluxePagerButton(IPostBackContainer container)
        {
            this.container = container;
        }

        protected override PostBackOptions GetPostBackOptions()
        {
            if (this.container != null)
                return this.container.GetPostBackOptions(this);

            return base.GetPostBackOptions();
        }
    }
}
