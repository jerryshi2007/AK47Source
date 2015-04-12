using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class QueryParamTest
    {
        public static void Output(this IEnumerable<SOARolePropertiesQueryParam> queryParams)
        {
            if (queryParams != null)
            {
                foreach(SOARolePropertiesQueryParam queryParam in queryParams)
                {
                    Console.WriteLine("Param Name: {0}, Param Value: {1}", queryParam.QueryName, queryParam.QueryValue);
                }
            }
        }
    }
}
