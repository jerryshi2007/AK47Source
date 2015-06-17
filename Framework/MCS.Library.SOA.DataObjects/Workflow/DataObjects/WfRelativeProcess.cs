﻿using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [ORTableMapping("WF.RELATIVE_PROCESSES")]
    [TenantRelativeObject]
    public sealed class WfRelativeProcess
    {

        [ORFieldMapping("RELATIVE_ID", PrimaryKey = true)]
        public string RelativeID { get; set; }

        [ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
        public string ProcessID { get; set; }

        [ORFieldMapping("DESCRIPTION")]
        public string Description { get; set; }

        [ORFieldMapping("RELATIVE_URL")]
        public string RelativeURL { get; set; }
    }

    public sealed class WfRelativeProcessCollection : EditableDataObjectCollectionBase<WfRelativeProcess>
    {

    }

}
