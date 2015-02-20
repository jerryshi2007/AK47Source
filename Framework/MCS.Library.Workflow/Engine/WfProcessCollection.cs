using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;

namespace MCS.Library.Workflow.Engine
{
    public class WfProcessCollection : WfKeyedCollectionBase<string, IWfProcess>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        public void Add(IWfProcess process)
        {
			this.InnerAdd(process);
        }

		public IWfProcess this[int index]
		{
			get
			{
				return base.InnerGet(index);
			}
		}

        public IWfProcess this[string processID]
        {
            get
            {
				IWfProcess wfProcess = this.InnerGet(processID);
				ExceptionHelper.FalseThrow(wfProcess != null, Resource.CanNotFoundProcessByID, processID);

				return wfProcess;
            }
        }

        protected override string GetKeyFromItem(IWfProcess data)
        {
            return data.ID;
        }
    }
}
