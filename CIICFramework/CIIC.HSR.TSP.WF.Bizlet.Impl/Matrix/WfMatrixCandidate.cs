using CIIC.HSR.TSP.WF.Bizlet.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixCandidate : IWfMatrixCandidate
    {
        private IWfMatrixParameterDefinition _WfMatrixParameterDefinition = new WfMatrixParameterDefinition();

        /* 沈峥注释
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid ID
        {
            get;
            set;
        }
         */

        /// <summary>
        /// 资源类型
        /// </summary>
        public string ResourceType
        {
            get;
            set;
        }

        /// <summary>
        /// 相关的动态角色
        /// </summary>
        public IWfMatrixParameterDefinition Candidate
        {
            get
            {
                return _WfMatrixParameterDefinition;
            }
            set
            {
                _WfMatrixParameterDefinition = value;
            }
        }

        public string ToExpression()
        {
            return Candidate.Name;
        }
    }
}
