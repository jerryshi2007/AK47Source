using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Library.Core;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
    [Serializable]
    public class GangedDataBindingItem
    {
        /// <summary>
        /// 绑定的控件ClientID
        /// </summary>
        public string ControlClientID
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定设置
        /// </summary>
        public string BindingSettings
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 数据绑定的集合类
    /// </summary>
    //[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    //[PersistenceMode(PersistenceMode.InnerProperty)]
    [Serializable]
    public class GangedDataBindingItemCollection : EditableKeyedDataObjectCollectionBase<string, GangedDataBindingItem>
    {
        protected override string GetKeyForItem(GangedDataBindingItem item)
        {
            return item.ControlClientID;
        }
    }
}
