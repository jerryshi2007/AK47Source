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
    /// ��������������
    /// </summary>
    /// <remarks>
    /// WfDescriptorBase�ǹ������Ļ��������а�����Key��Name��Description
    /// �Ǵӹ�������������Ļ������ԣ�������������Key��Name��Description���̳��ڴˡ�
    /// </remarks>
    /// <scholiast>������</scholiast>
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
        /// �����̸���Keyֵ
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>��������������Ҫ��Keyֵ�����ɴ˻�ã�Process��Activity��Transition��</remarks>
        /// <scholiast>������</scholiast>
        protected WfDescriptorBase(string key)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

            _Key = key;
        }

        /// <summary>
        /// ������Description���Է�����
        /// </summary>
        /// <remarks>������Description���Է�����</remarks>
        /// <scholiast>������</scholiast>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// ������Name���Է�����
        /// </summary>
        /// <remarks>������Name���Է�����</remarks>
        /// <scholiast>������</scholiast>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// ������Key���Է�����
        /// </summary>
        /// <remarks>������Key���Է�����</remarks>
        /// <scholiast>������</scholiast>
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
    /// T���Ͷ���ļ���
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    /// <remarks>���̶��弯�ϻ���</remarks>
    /// <scholiast>������</scholiast>
    [Serializable]
	public abstract class WfDescriptorCollectionBase<T> : WfCollectionBase<T> where T : IWfDescriptor
	{
		private Hashtable _InnerDict = new Hashtable();

		public WfDescriptorCollectionBase()
		{
		}

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="index">����</param>
        /// <returns>���ذ��������ʵ�����</returns>
        public T this[int index]
        {
            get
            {
				return InnerGet(index);
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>���ذ�Keyֵ���ʵ�����</returns>
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
		/// ���Ƶ��ֵ���
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
		/// ���ֶ��и���
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
        /// ���� CollectionBase ʵ���в�����Ԫ��֮ǰִ�������Զ�����̡�
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">value</param>
        /// <remarks>������̣��������ݵĺ����ԣ�����_InnerDict������ݣ����ִ��base.OnInsert()</remarks>
        /// <scholiast>������</scholiast>
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
