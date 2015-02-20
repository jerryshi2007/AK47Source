using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfBranchProcessInfoCollection : WfKeyedCollectionBase<string, WfBranchProcessInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processInfo"></param>
        public void Add(WfBranchProcessInfo processInfo)
        {
            this.InnerAdd(processInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processInfo"></param>
        public void Remove(WfBranchProcessInfo processInfo)
        {
            this.InnerRemove(processInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WfBranchProcessInfo this[int index]
        {
            get
            {
				return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public WfBranchProcessInfo this[string processID]
        {
            get
            {
				return this.InnerGet(processID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(WfBranchProcessInfo data)
        {
            return data.Process.ID;
        }

        public bool AllBranchesCompleted()
        {
            bool flag = true;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (WfBranchProcessInfo processInfo in this)
				{
					if (processInfo.Process.Status != WfProcessStatus.Completed &&
						processInfo.Process.Status != WfProcessStatus.Aborted)
					{
						flag = false;
						break;
					}
				}

				return flag;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
        }

        public bool AnyoneBranchProcessCompleted()
        {
            bool flag = false;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (WfBranchProcessInfo processInfo in this)
				{
					if (processInfo.Process.Status == WfProcessStatus.Completed)
					{
						flag = true;
						break;
					}
				}

				return flag;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
        }

        public bool SpecificBranchesProcessCompleted()
        {
            List<WfBranchProcessInfo> specificBranches = new List<WfBranchProcessInfo>();

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (WfBranchProcessInfo processInfo in this)
				{
					if (processInfo.IsSpecificProcess)
					{
						specificBranches.Add(processInfo);
					}
				}

				bool flag = true;

				if (specificBranches.Count == 0)
					flag = AllBranchesCompleted();
				else
				{
					foreach (WfBranchProcessInfo processInfo in specificBranches)
					{
						if (processInfo.Process.Status != WfProcessStatus.Completed)
						{
							flag = false;
							break;
						}
					}
				}

				return flag;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
        }
    }
}
