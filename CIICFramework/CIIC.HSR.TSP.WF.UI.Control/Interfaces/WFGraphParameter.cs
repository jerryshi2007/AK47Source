using CIIC.HSR.TSP.WF.UI.Control.Controls.Graph;
using MCS.Library.WF.Contracts.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    public class WFGraphParameter : WFParameterWithResponseBase
    {
        protected override void InternalExecute(ResponseData response)
        {
            WFUIRuntimeContext runtime = WFUIRuntimeContext.InitByProcessID(ProcessId);
            if (runtime == null || runtime.Process == null)
            {
                return;
            }
            response.ProcessInfo = runtime.Process;         
        } 
    }
}
