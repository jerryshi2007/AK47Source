using System;
using System.Text;
using System.Collections.Generic;                  
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Data;

namespace MCS.Library.Workflow.Engine
{
    [Serializable]
    public class WfActivity : WfActivityBase, IWfActivity
    {
        #region ISerializable ≥…‘±

        protected WfActivity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
        protected WfActivity()
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
        public WfActivity(IWfActivityDescriptor descriptor)
            : base(descriptor)
        {
        }
    }
}
