using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{
    public class WfClientDynamicResourceDescriptor : WfClientResourceDescriptor
    {
        private WfClientConditionDescriptor _Condition = null;

        public WfClientDynamicResourceDescriptor()
        {
        }

        public WfClientDynamicResourceDescriptor(string name, string expression)
        {
            this.Name = name;
            this.Condition.Expression = expression;
        }

        /// <summary>
        /// 动态角色的名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public WfClientConditionDescriptor Condition
        {
            get
            {
                if (this._Condition == null)
                    this._Condition = new WfClientConditionDescriptor();

                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
        }
    }
}
