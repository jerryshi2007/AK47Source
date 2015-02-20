using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    [Serializable]
    public class WfTransition : IWfTransition, ISerializable
    {
		private string _ID = UuidHelper.NewUuidString();
        private IWfActivity _FromActivity = null;
        private IWfActivity _ToActivity = null;
        protected DateTime _StartTime = DateTime.MinValue;
        private bool _IsAborted = false;
        

        #region ISerializable 成员

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			info.AddValue("ID", this._ID);
            info.AddValue("FromActivity", this._FromActivity, typeof(WfActivityBase));
            info.AddValue("ToActivity", this._ToActivity, typeof(WfActivityBase));
			info.AddValue("IsAborted", this._IsAborted);
			info.AddValue("StartTime", this._StartTime);
        }

        protected WfTransition(SerializationInfo info, StreamingContext context)
        {
			this._ID = info.GetString("ID");
            this._FromActivity = (WfActivityBase)info.GetValue("FromActivity", typeof(WfActivityBase));
            this._ToActivity = (WfActivityBase)info.GetValue("ToActivity", typeof(WfActivityBase));
			this._IsAborted = info.GetBoolean("IsAborted");
			this._StartTime = info.GetDateTime("StartTime");
        }

        #endregion

        protected WfTransition()
        {
        }

        public WfTransition(IWfTransitionDescriptor descriptor)
        {
            this._StartTime = DateTime.Now;
        }


        #region 公共属性
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public IWfActivity FromActivity
        {
            get
            {
                return this._FromActivity;
            }
            set 
            {
                this._FromActivity = value;
            }
        }

        public IWfActivity ToActivity
        {
            get
            {
                return this._ToActivity;
            }
             set 
            {
                this._ToActivity = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
            }
        }

        public bool IsAborted
        {
            get 
            {
                return _IsAborted;
            }
        }
        #endregion

    }
}
