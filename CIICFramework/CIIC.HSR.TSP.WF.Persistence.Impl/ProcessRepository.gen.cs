// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier

using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.Persistence.Contract;
using System;

namespace CIIC.HSR.TSP.WF.Persistence.Impl
{ 
    // TSPWF_Process
    public partial class ProcessRepository : EFRepository<ProcessBO>, IProcessRepository
    {
  
    }
}
