using System.Collections.Generic;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public class LevelItem 
    {        

        public string ConditionText { get; set; }

        public string ConditionValue { get; set; }

        private List<LevelItem> _conditionItems;

        public List<LevelItem> ConditionItems
        {
            get
            {
                if (null != _conditionItems)
                {
                    return _conditionItems;
                }
                _conditionItems = new List<LevelItem>(5)
                                      {
                                          new LevelItem {ConditionValue = "AnyLevel", ConditionText = "不限"},
                                          new LevelItem {ConditionValue = "Excellent", ConditionText = "优秀"},
                                          new LevelItem {ConditionValue = "Qualified", ConditionText = "合格"},
                                          new LevelItem {ConditionValue = "ToExamine", ConditionText = "待考察"},
                                          new LevelItem {ConditionValue = "Alternatives", ConditionText = "备选"}
                                      };

                return _conditionItems;
            }
            set { _conditionItems = value; }
        }

        public bool IsAdvanced { get; set; }        
    }
}