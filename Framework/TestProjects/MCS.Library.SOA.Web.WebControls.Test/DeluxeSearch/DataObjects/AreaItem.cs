using System.Collections.Generic;
using System.Data;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{

    public class AreaItem
    {

        public AreaItem()
        {           
        }    

        public string AreaName { get; set; }

        public string AreaValue { get; set; }

        private List<AreaItem> _conditionItems;

        public List<AreaItem> ConditionItems
        {
            get
            {
                if (null != _conditionItems)
                {
                    return _conditionItems;
                }
                _conditionItems = new List<AreaItem>(5)
                                      {                                       
                                          new AreaItem {AreaValue = "1", AreaName = "集团", IsAdvanced=true},
                                          new AreaItem {AreaValue = "2", AreaName = "华东地区", IsAdvanced=true},
                                          new AreaItem {AreaValue = "3", AreaName = "华南地区", IsAdvanced=true},
                                          new AreaItem {AreaValue = "4", AreaName = "华中地区",IsAdvanced = false},
                                          new AreaItem {AreaValue = "5", AreaName = "华北地区",IsAdvanced = false}                                        
                                      };

                return _conditionItems;
            }
            set { _conditionItems = value; }
        }

        public bool IsAdvanced { get; set; }

    }
}