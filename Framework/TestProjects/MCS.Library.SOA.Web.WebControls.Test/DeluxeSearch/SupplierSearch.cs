using System;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch
{
    [Serializable]
    public class SupplierSearch
    {
        [ConditionMapping("SupplierRegionalCode")]
        public string SupplierRegionalCode { get; set; }

        [ConditionMapping("Status")]
        public string Status { get; set; }      
    }       
}