using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.Principal;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIICWFControlTest.Bridges
{
    public class DeluxeWfUserContext : IWFUserContext
    {
        public WFUserContext GetUser()
        {
            WFUserContext result = new WFUserContext();

            if (DeluxePrincipal.IsAuthenticated)
                result.WfClientUser = (WfClientUser)DeluxeIdentity.CurrentUser.ToClientOguObject();

            return result;
        }
    }
}