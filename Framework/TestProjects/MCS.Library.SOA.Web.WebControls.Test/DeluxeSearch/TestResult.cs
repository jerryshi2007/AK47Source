using System.Collections.Generic;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public class TestResult
    {
        public List<AreaItem> GetAreaItem()
        {
            return new AreaItem().ConditionItems;
        }
        public List<TypeItem> GetTypeItem()
        {
            return new TypeItem().ConditionItems;
        }
    }
}