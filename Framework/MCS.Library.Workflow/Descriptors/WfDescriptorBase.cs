using System;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 工作流基本属性
    /// </summary>
    /// <remarks>
    /// WfDescriptorBase是工作流的基础，其中包含的Key、Name、Description
    /// 是从工作留抽象出来的基本属性，工作流中所有Key、Name、Description均继承于此。
    /// </remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
	[DebuggerDisplay("Key = {_Key}")]
	[DebuggerDisplay("Name = {_Name}")]
	[DebuggerDisplay("Description = {_Description}")]
    public abstract class WfDescriptorBase : IWfDescriptor, ISerializable
    {
        private string _Key = string.Empty;
        private string _Name = string.Empty;
        private string _Description = string.Empty;
        private bool _Enabled = true;

        /// <summary>
        /// 
        /// </summary>
        protected WfDescriptorBase()
        {
        }

        /// <summary>
        /// 给流程赋予Key值
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>工作流中所有需要的Key值都是由此获得（Process、Activity、Transition）</remarks>
        /// <scholiast>徐照宇</scholiast>
        protected WfDescriptorBase(string key)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            _Key = key;
        }

        /// <summary>
        /// 工作流Description属性访问器
        /// </summary>
        /// <remarks>工作流Description属性访问器</remarks>
        /// <scholiast>徐照宇</scholiast>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// 工作流Name属性访问器
        /// </summary>
        /// <remarks>工作流Name属性访问器</remarks>
        /// <scholiast>徐照宇</scholiast>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// 工作流Key属性访问器
        /// </summary>
        /// <remarks>工作流Key属性访问器</remarks>
        /// <scholiast>徐照宇</scholiast>
        public virtual string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        public bool Enabled
        {
            get 
            { 
                return _Enabled; 
            }
            set 
            {
                _Enabled = value;
            }
        }

        #region ISerializable Members
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", _Key);
            info.AddValue("Name", _Name);
            info.AddValue("Description", _Description);
            info.AddValue("Enabled", _Enabled);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfDescriptorBase(SerializationInfo info, StreamingContext context)
        {
            this._Key = info.GetString("Key");
            this._Name = info.GetString("Name");
            this._Description = info.GetString("Description");
            this._Enabled = info.GetBoolean("Enabled");
        }

        #endregion
    }

    /// <summary>
    /// T类型对象的集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <remarks>流程定义集合基类</remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
	public abstract class WfDescriptorCollectionBase<T> : WfCollectionBase<T> where T : IWfDescriptor
	{
		private Hashtable _InnerDict = new Hashtable();

		public WfDescriptorCollectionBase()
		{
		}

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>返回按索引访问的类型</returns>
        public T this[int index]
        {
            get
            {
				return InnerGet(index);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>返回按Key值访问的类型</returns>
        public T this[string key]
        {
            get
            {
				this.RWLock.AcquireReaderLock(lockTimeout);
				try
				{
					return (T)_InnerDict[key];
				}
				finally
				{
					this.RWLock.ReleaseReaderLock();
				}
            }
        }

		/// <summary>
		/// 复制到字典中
		/// </summary>
		/// <param name="dict"></param>
		public void CopyTo(IDictionary<string, T> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				dict.Clear();

				foreach (DictionaryEntry entry in this._InnerDict)
					dict.Add((string)entry.Key, (T)entry.Value);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 从字段中复制
		/// </summary>
		/// <param name="dict"></param>
		public void CopyFrom(IDictionary<string, T> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				this.Clear();
				this._InnerDict.Clear();

				foreach (var entry in dict)
					List.Add(entry.Value);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        protected void InnerRemove(string key)
        {
			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				T data = (T)_InnerDict[key];

				if (data != null)
					List.Remove(data);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 在向 CollectionBase 实例中插入新元素之前执行其他自定义进程。
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">value</param>
        /// <remarks>处理过程：检验数据的合理性，并向_InnerDict添加数据，最后执行base.OnInsert()</remarks>
        /// <scholiast>徐照宇</scholiast>
        protected override void OnInsert(int index, object value)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				T data = (T)value;

				ExceptionHelper.TrueThrow<ArgumentException>(_InnerDict.ContainsKey(data.Key),
					Resource.DuplicateDescriptorKey, typeof(T).Name, data.Key);

				_InnerDict.Add(data.Key, data);
				base.OnInsert(index, data);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, object value)
        {
			this.RWLock.AcquireWriterLock(lockTimeout);

			try
			{
				base.OnRemove(index, value);

				T data = (T)value;
				_InnerDict.Remove(data.Key);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnClearComplete()
        {
			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				base.OnClearComplete();
				_InnerDict.Clear();
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }
    }
}
