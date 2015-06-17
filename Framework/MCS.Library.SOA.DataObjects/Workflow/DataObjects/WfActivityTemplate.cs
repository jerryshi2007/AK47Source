using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 活动点模板
    /// </summary>
    [Serializable]
    [XElementSerializable]
    [ORTableMapping("WF.ACTIVITY_TEMPLATE")]
    [TenantRelativeObject]
    public class WfActivityTemplate
    {
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        [ORFieldMapping("Name")]
        public string Name { get; set; }

        [ORFieldMapping("CATEGORY")]
        public string Category { get; set; }

        [ORFieldMapping("CONTENT")]
        public string Content { get; set; }

        [ORFieldMapping("CREATOR_ID")]
        public string CreatorID { get; set; }

        [ORFieldMapping("CREATOR_NAME")]
        public string CreatorName { get; set; }

        [ORFieldMapping("CREATE_TIME")]
        public DateTime CreateTime { get; set; }

        [ORFieldMapping("AVAILABLE")]
        public bool Available { get; set; }
    }


    [Serializable]
    [XElementSerializable]
    public class WfActivityTemplateCollection : EditableDataObjectCollectionBase<WfActivityTemplate>
    {
    }
}
