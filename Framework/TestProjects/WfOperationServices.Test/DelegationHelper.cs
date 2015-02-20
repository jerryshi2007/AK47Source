using MCS.Library.Core;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WfOperationServices.Test
{
   public class DelegationHelper
    {
       
        public static WfClientDelegationCollection PrepareDelegations(string userID)
        {
          
            WfClientDelegationCollection delegations = new WfClientDelegationCollection();
            WfClientDelegation delegation = PrepareDelegation();
            delegation.SourceUserID = userID;
            delegation.SourceUserName = "1_" + delegation.SourceUserName;
            WfClientProcessDescriptorServiceProxy.Instance.UpdateUserDelegation(delegation);           
            delegations.Add(delegation);

            delegation = PrepareDelegation();
            delegation.SourceUserID = userID;
            delegation.SourceUserName = "2_" + delegation.SourceUserName;
            WfClientProcessDescriptorServiceProxy.Instance.UpdateUserDelegation(delegation);
            delegations.Add(delegation);

            return delegations;
        }

       public static WfClientDelegation PrepareDelegation()
       {
            WfClientDelegation delegation = new WfClientDelegation();

            delegation.SourceUserID = UuidHelper.NewUuidString();
            delegation.DestinationUserID = UuidHelper.NewUuidString();

            delegation.SourceUserName = "Source UserName";
            delegation.DestinationUserName = "Destination UserName";
            delegation.Enabled =true;
            delegation.StartTime = new DateTime(2014, 11, 1, 0, 0, 0, DateTimeKind.Utc);
            delegation.EndTime = new DateTime(2014, 11, 11, 0, 0, 0, DateTimeKind.Local);

            delegation.ApplicationName = "WF ApplicationName";
            delegation.ProgramName = "WF ProgramName";
            delegation.TenantCode = UuidHelper.NewUuidString();

            return delegation;
       }

    }
}
