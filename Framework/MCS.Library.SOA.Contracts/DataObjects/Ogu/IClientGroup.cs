using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
    
    public interface IClientGroup : IClientOguObject
    {
        // Summary:
        //     组内的人员
        ClientOguObjectCollection<ClientOguUser> Members { get; set; }
    }
}
