using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.Services;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
	public class TSPWFAutoGenBizletsIoCConfigure : IIoCConfigure
	{
		public void Configure(IIoCContainer container)
		{
            container.Register<IWfMatrixStorageManager, WfMatrixStorageManager>(alwaysNew: true);  
		}
	}
}
