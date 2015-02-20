using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{
    [DataContract]
    public class WfClientActionParams
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="context"></param>
        public WfClientActionParams(WfClientProcessActionContext context)
        {
            this.Context = context;
        }

        public WfClientActionParams()
        {  
        }
        [DataMember]
        public WfClientProcessActionContext Context
        {
            get;
            set;
        }
    }
    [DataContract(IsReference=true)]
    public class ClientUserTaskActionBase { }
}
