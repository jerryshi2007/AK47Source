using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.Models
{
    public class TaskModel
    {
        public PagedCollection<USER_TASKBO_PROCESS> UncompletedTask { get; set; }
        public PagedCollection<USER_TASKBO_PROCESS> CompletedTask { get; set; }
    }
}