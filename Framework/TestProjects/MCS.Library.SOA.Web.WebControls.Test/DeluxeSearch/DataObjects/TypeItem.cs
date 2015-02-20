using System.Collections.Generic;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test
{
    public class TypeItem 
    {

        #region IConditionItem 成员    

        public string TypeName { get; set; }

        public string TypeValue { get; set; }

        private List<TypeItem> _conditionItems;
        public List<TypeItem> ConditionItems
        {
            get
            {
                if (null != _conditionItems)
                {
                    return _conditionItems;
                }
                _conditionItems = new List<TypeItem>(4)
                                      {                                          
                                          new TypeItem {TypeValue = "1", TypeName = "工程类"},
                                          new TypeItem {TypeValue = "2", TypeName = "货物类"},        
                                          new TypeItem {TypeValue = "3", TypeName = "服务类"}
                                      };

                return _conditionItems;
            }
            set { _conditionItems = value; }
        }

        public bool IsAdvanced { get; set; }
        #endregion
    }
}