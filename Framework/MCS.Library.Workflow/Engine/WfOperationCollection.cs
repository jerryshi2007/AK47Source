using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfOperationCollection : WfKeyedCollectionBase<string, IWfOperation>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opt"></param>
        public void Add(IWfOperation opt)
        {
			this.InnerAdd(opt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IWfOperation this[int index]
        {
            get
            {
				return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IWfOperation this[string opID]
        {
            get
            {
				return this.InnerGet(opID);
            }
        }

		/// <summary>
		/// 根据OperationDescriptor的Key来查找匹配的第一个Operation
		/// </summary>
		/// <param name="opKey"></param>
		/// <returns></returns>
		public IWfOperation FindOperationByDescriptor(string opKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(opKey, "opKey");

			IWfOperation result = null;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (IWfOperation op in this)
				{
					if (string.Compare(op.Descriptor.Key, opKey, true) == 0)
					{
						result = op;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 根据OperationDescriptor来查找匹配的第一个Operation
		/// </summary>
		/// <param name="opDesp"></param>
		/// <returns></returns>
		public IWfOperation FindOperationByDescriptor(IWfOperationDescriptor opDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(opDesp != null, "opDesp");

			return FindOperationByDescriptor(opDesp.Key);
		}

		/// <summary>
		/// 是否所有的分支流程都结束了
		/// </summary>
		/// <returns></returns>
		public bool AllOperationsCompleted()
		{
			return TrueForAll(delegate(IWfOperation op) { return op.Branches.AllBranchesCompleted(); });
		}

		internal void Remove(IWfOperation operation)
		{
			InnerRemove(operation);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(IWfOperation data)
        {
            return data.ID;
        }
    }
}
