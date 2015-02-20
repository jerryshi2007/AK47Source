

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Configuration file:     "TSP.WF\CIICFramework\CIIC.HSR.TSP.WF.Persistence.Impl\app.config"
//     Connection String Name: "Default"
//     Connection String:      "data source=172.16.8.52;initial catalog=HSRMaster;persist security info=True;user id=hsr_sa;password=**zapped**;App=EntityFramework"

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
}


namespace CIIC.HSR.TSP.WF.Persistence.Impl
{ 

       
    public class TSPWFRepositoriesIoCConfig : CIIC.HSR.TSP.IoC.IIoCConfigure
	{

		public void Configure(IoC.IIoCContainer container)
		{
                           
            	container.Register<IGENERIC_OPINIONSRepository, GENERIC_OPINIONSRepository>(alwaysNew: true);
                               
            	container.Register<IProcessRepository, ProcessRepository>(alwaysNew: true);
                               
            	container.Register<IUSER_ACCOMPLISHED_TASKRepository, USER_ACCOMPLISHED_TASKRepository>(alwaysNew: true);
                               
            	container.Register<IUSER_OPERATION_LOGRepository, USER_OPERATION_LOGRepository>(alwaysNew: true);
                               
            	container.Register<IUSER_OPERATION_TASKS_LOGRepository, USER_OPERATION_TASKS_LOGRepository>(alwaysNew: true);
                               
            	container.Register<IUSER_TASKRepository, USER_TASKRepository>(alwaysNew: true);
    		}
	}
}


