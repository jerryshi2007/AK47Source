using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    public class WfMatrixProcessDescriptorCreateParams
    {
       /// <summary>
       /// 流程编码
       /// </summary>
        public string Key { set; get; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { set; get; }
    }
}
