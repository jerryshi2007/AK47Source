// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier
using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.Models;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace CIIC.HSR.TSP.WF.BizObject
{
    public interface ITSPWFDbContext : IDisposable
    {
        IDbSet<GENERIC_OPINIONSBO> GENERIC_OPINIONSBO { get; set; } // TSPWF_GENERIC_OPINIONS
        IDbSet<ProcessBO> ProcessBOes { get; set; } // TSPWF_Process
        IDbSet<USER_ACCOMPLISHED_TASKBO> USER_ACCOMPLISHED_TASKBO { get; set; } // TSPWF_USER_ACCOMPLISHED_TASK
        IDbSet<USER_OPERATION_LOGBO> USER_OPERATION_LOGBO { get; set; } // TSPWF_USER_OPERATION_LOG
        IDbSet<USER_OPERATION_TASKS_LOGBO> USER_OPERATION_TASKS_LOGBO { get; set; } // TSPWF_USER_OPERATION_TASKS_LOG
        IDbSet<USER_TASKBO> USER_TASKBO { get; set; } // TSPWF_USER_TASK

        int SaveChanges();
    }

}
