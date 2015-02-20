using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MCS.Library.Workflow.Engine
{
    [Serializable]
    public abstract class WfRunningObjectBase<T> : ISerializable where T : IWfDescriptor
    {
        private string _ID = string.Empty;
        private T _Descriptor;

        #region ISerializable ≥…‘±

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", this._ID);
            info.AddValue("Descriptor", this._Descriptor, typeof(T));
        }

        protected WfRunningObjectBase(SerializationInfo info, StreamingContext context)
        {
            this._ID = (string)info.GetValue("ID", typeof(string));
            this._Descriptor = (T)info.GetValue("Descriptor", typeof(T));
        }

        #endregion

        protected WfRunningObjectBase()
        {
        }

        protected WfRunningObjectBase(T descriptor)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(descriptor != null, "descriptor");

            _Descriptor = descriptor;
			_ID = UuidHelper.NewUuidString();
        }

        public T Descriptor
        {
            get
            {
                return _Descriptor;
            }
            protected set
            {
                _Descriptor = value;
            }
        }

        public string ID
        {
            get 
            { 
                return _ID; 
            }
            protected set
            {
                this._ID = value;
            }
        }
    }
}
