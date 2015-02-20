using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 复制主线活动点的上下文对象
    /// </summary>
    internal class WfCopyMainStreamContext
    {
        private WfCopyMainStreamActivityNode _StartActivityDescriptor = null;
        private IWfActivityDescriptor _EndActivityDescriptor = null;
        private Dictionary<string, IWfActivityDescriptor> _ClonedActivities = new Dictionary<string, IWfActivityDescriptor>();
        private Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> _ElapsedTransitions = new Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor>();
        private bool _IsMainStream = false;
        private IWfTransitionDescriptor _EntryTransition = null;

        public WfCopyMainStreamContext(IWfActivityDescriptor startActDesp, IWfActivityDescriptor endActDesp, IWfTransitionDescriptor entryTransition, bool isMainStream)
        {
            this._IsMainStream = isMainStream;
            this._StartActivityDescriptor = new WfCopyMainStreamActivityNode(startActDesp, startActDesp, true, false);
            this._EndActivityDescriptor = endActDesp;
            this._EntryTransition = entryTransition;
        }

        public void AddElapsedTransition(IWfTransitionDescriptor transition)
        {
            if (this._ElapsedTransitions.ContainsKey(transition) == false)
                this._ElapsedTransitions.Add(transition, transition);
        }

        public bool IsElapsedTransition(IWfTransitionDescriptor transition)
        {
            return this._ElapsedTransitions.ContainsKey(transition);
        }

        public IWfActivityDescriptor EndActivityDescriptor
        {
            get
            {
                return this._EndActivityDescriptor;
            }
        }

        public IWfTransitionDescriptor EntryTransition
        {
            get
            {
                return this._EntryTransition;
            }
        }

        public bool IsMainStream
        {
            get
            {
                return this._IsMainStream;
            }
        }

        public WfCopyMainStreamActivityNode StartActivityDescriptor
        {
            get
            {
                return this._StartActivityDescriptor;
            }
        }

        public Dictionary<string, IWfActivityDescriptor> ClonedActivities
        {
            get
            {
                return this._ClonedActivities;
            }
        }
    }
}
