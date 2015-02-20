using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 与流程实例有关的
    /// </summary>
    [Serializable]
	public abstract class WfKeyedCollectionBase<TKey, TValue> : WfCollectionBase<TValue>
    {
		private IDictionary _InnerDict = new Hashtable();

		public bool ContainsKey(TKey key)
		{
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return InnerDict.Contains(key);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		protected virtual TValue InnerGet(TKey key)
		{
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return (TValue)InnerDict[key];
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

        /// <summary>
        /// 插入对象处理
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(value == null, "value");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				TKey key = GetKeyFromItem((TValue)value);
				TValue data = (TValue)value;

				ExceptionHelper.TrueThrow<ArgumentException>(InnerDict.Contains(key),
					Resource.DuplicateDescriptorKey, typeof(TValue).Name, key);

				InnerDict.Add(key, data);
				base.OnInsert(index, data);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        protected override void OnRemove(int index, object value)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(value == null, "value");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				TKey key = GetKeyFromItem((TValue)value);
				TValue data = (TValue)value;

				ExceptionHelper.FalseThrow<ArgumentException>(InnerDict.Contains(key),
					Resource.InexistenceKey, typeof(TValue).Name, key);

				InnerDict.Remove(key);
				base.OnRemove(index, data);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 根据Value得到Key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract TKey GetKeyFromItem(TValue data);

        /// <summary>
        /// 
        /// </summary>
        protected IDictionary InnerDict
        {
            get
            {
                return this._InnerDict;
            }
        }
    }
}
