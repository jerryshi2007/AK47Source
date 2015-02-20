using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.Workflow.Engine;
using MCS.Library.Core;
using System.Diagnostics;
using System.Collections;

namespace MCS.Library.Workflow.Services
{
    [Serializable]
    public abstract class WorkItemBase : ISerializable
    {
        private Guid _CorrelationID = Guid.Empty;
        private Hashtable _WIContext = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        public WorkItemBase()
        {
        }

        public Guid CorrelationID
        {
            get
            {
                return this._CorrelationID;
            }
            protected set
            {
                this._CorrelationID = value;
            }
        }

        public Hashtable WIContext
        {
            get
            {
                return _WIContext;
            }
        }

        protected WorkItemBase(SerializationInfo info, StreamingContext context)
        {
            _CorrelationID = (Guid)info.GetValue("CorrelationID", typeof(Guid));
            _WIContext = (Hashtable)info.GetValue("WIContext", typeof(Hashtable));
        }


        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{

            info.AddValue("CorrelationID", this._CorrelationID);

			info.AddValue("WIContext", this._WIContext, typeof(Hashtable));
		}

        #endregion
    }
}
