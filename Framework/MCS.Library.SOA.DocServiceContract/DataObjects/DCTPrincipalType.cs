using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.DocServiceContract
{
     [Serializable]
   //[DataContract]
    public enum DCTPrincipalType
    {
        DefaultValue = 0,
        User = 1,
        SharePointGroup = 8,
    }
}
