using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 流程创建器的基类，以后创建不同类型的流程时，可以重载此类
    /// </summary>
    public abstract class WfMatrixProcessBuilderBase
    {
        public abstract IWfMatrixProcess BuildProcess(string processKey);

        protected IWfMatrixActivity CreateActivity(WfMaxtrixActivityType activityType)
        {
            IWfMatrixActivity activity = new WfMatrixActivity(activityType);

            return activity;
        }
    }
}
