using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    public class WFUserContext
    {
        private WfClientUser _WfClientUser = new WfClientUser();

        public WfClientUser WfClientUser
        {
            get { return _WfClientUser; }
            set { _WfClientUser = value; }
        }
        public string TenantCode { get; set; }
    }
}
