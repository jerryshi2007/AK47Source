using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
    [Serializable]
    [ORTableMapping("Biz.ConditionalGroups")]
    public class ConditionalGroup
    {
        [ORFieldMapping("GroupID", PrimaryKey = true)]
        public string GroupID
        {
            get;
            set;
        }

        [ORFieldMapping("Name")]
        public string Name
        {
            get;
            set;
        }

        [ORFieldMapping("Description")]
        public string Description
        {
            get;
            set;
        }

        [ORFieldMapping("Condition")]
        public string Condition
        {
            get;
            set;
        }

        [ORFieldMapping("CalculateTime")]
        public DateTime CalculateTime
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ConditionalGroupCollection : EditableDataObjectCollectionBase<ConditionalGroup>
    {
    }
}
