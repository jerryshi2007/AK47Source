using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract(IsReference = true)]
    public class WfClientDynamicResourceDescriptor : WfClientResourceDescriptor
    {
        [DataMember]
        public string Name { get; set; }
        WfClientConditionDescriptor _Condition = null;
        /// <summary>
        /// 动态角色的名称
        /// </summary>
        
       [DataMember]
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
