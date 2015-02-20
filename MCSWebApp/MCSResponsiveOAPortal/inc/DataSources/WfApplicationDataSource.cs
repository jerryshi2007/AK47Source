using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCSResponsiveOAPortal.DataSources
{
    [DataObject]
    public class WfApplicationDataSource
    {
        public static WfApplicationCollection GetAllApplications()
        {
            return WfApplicationAdapter.Instance.LoadAll();
        }
    }
}