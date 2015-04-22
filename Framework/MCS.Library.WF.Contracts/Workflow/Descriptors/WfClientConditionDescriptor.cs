using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    [DataContract]
    [Serializable]
    public class WfClientConditionDescriptor
    {
        public WfClientConditionDescriptor()
        {
        }

        public WfClientConditionDescriptor(string expression)
        {
            this.Expression = expression;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 是否条件为空
        /// </summary>
        [ScriptIgnore]
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Expression);
            }
        }

        public WfClientConditionDescriptor Clone()
        {
            WfClientConditionDescriptor condition = new WfClientConditionDescriptor(this.Expression);

            return condition;
        }
    }
}
