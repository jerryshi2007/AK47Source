using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{

    public class WfMatrixParameterDefinition : IWfMatrixParameterDefinition
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        //public Guid Id
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// 内部名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue
        {
            get;
            set;
        }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }


        public Common.ParaType ParameterType
        {
            get;
            set;
        }
    }
 
}
