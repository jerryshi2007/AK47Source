using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    public class ResponseData
    {
        public object BusinessData { get; set; }
        public WfClientProcessInfo ProcessInfo { get; set; }
    }
}
