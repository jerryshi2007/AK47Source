using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CIIC.HSR.TSP.Services;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
namespace CIIC.HSR.TSP.WF.Ctrl.Test.Models
{
    public class WFUserContextTest:IWFUserContext
    {

        public WFUserContext GetUser()
        {
            WFUserContext clientUser = new WFUserContext();
            clientUser.WfClientUser.ID = "4EF6BE7E-9300-416D-B390-EBB859A6D05B";
            clientUser.WfClientUser.Name = "曹节";
            clientUser.WfClientUser.DisplayName = "曹节1";
            clientUser.TenantCode = "D5561180-7617-4B67-B68B-1F0EA604B509";
            return clientUser;
        }
    }
}