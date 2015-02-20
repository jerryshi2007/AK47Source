using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixEmptyProcessBuilder : WfMatrixProcessBuilderBase
    {
        public static readonly WfMatrixProcessBuilderBase Instance = new WfMatrixEmptyProcessBuilder();

        public WfMatrixEmptyProcessBuilder()
        {
        }

        public override IWfMatrixProcess BuildProcess(string processKey)
        {
            IWfMatrixProcess process = new WfMatrixProcess();
            process.Key = processKey;

            process.Activities.Add(this.CreateActivity(WfMaxtrixActivityType.InitialActivity));
            process.Activities.Add(this.CreateActivity(WfMaxtrixActivityType.CompletedActivity));

            return process;
        }
    }
}
