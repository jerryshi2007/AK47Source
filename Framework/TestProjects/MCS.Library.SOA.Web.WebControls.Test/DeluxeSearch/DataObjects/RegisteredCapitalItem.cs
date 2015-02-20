using System.Collections.Generic;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public class RegisteredCapitalItem 
    {               

        public string ConditionText { get; set; }

        public string ConditionValue { get; set; }

        private List<RegisteredCapitalItem> _conditionItems;

        public List<RegisteredCapitalItem> ConditionItems
        {
            get
            {
                if (null != _conditionItems)
                {
                    return _conditionItems;
                }
                _conditionItems = new List<RegisteredCapitalItem>(5)
                                      {
                                          new RegisteredCapitalItem {ConditionValue = "Any", ConditionText = "不限"},
                                          new RegisteredCapitalItem {ConditionValue = "100", ConditionText = "1~100"},
                                          new RegisteredCapitalItem {ConditionValue = "500", ConditionText = "100~500"},
                                          new RegisteredCapitalItem {ConditionValue = "1000", ConditionText = "500~1000"},
                                          new RegisteredCapitalItem {ConditionValue = "L1000", ConditionText = "1000以上"}
                                      };

                return _conditionItems;
            }
            set { _conditionItems = value; }
        }


        public bool IsAdvanced { get; set; }        
    }
}