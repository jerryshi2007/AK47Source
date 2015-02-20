using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using System.Reflection;
using MCS.Library.Workflow.Properties;
using System.Collections;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 
	/// </summary>
	public enum DataType
	{
		String,
		Int,
		Double,
		Float,
		Boolean,
		DateTime
	}

	/// <summary>
	/// 变量列表
	/// </summary>
	/// <remarks></remarks>
	/// <scholiast>徐照雨</scholiast>
	[Serializable]
	public class WfVariableDescriptor : WfDescriptorBase
	{
		private DataType _OriginalType = DataType.String;
		private string _OriginalValue = string.Empty;

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		public WfVariableDescriptor()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public WfVariableDescriptor(string key)
			: base(key)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="originalValue"></param>
		public WfVariableDescriptor(string key, string originalValue)
			: this(key, originalValue, DataType.String)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="dataType"></param>
		public WfVariableDescriptor(string key, string originalValue, DataType dataType)
			: this(key)
		{
			this._OriginalType = dataType;
			this._OriginalValue = originalValue;
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public DataType OriginalType
		{
			get
			{
				return this._OriginalType;
			}
			set
			{
				this._OriginalType = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string OriginalValue
		{
			get
			{
				return this._OriginalValue;
			}
			set
			{
				this._OriginalValue = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public System.Type ActualType
		{
			get
			{
				return GetActualType(this._OriginalType);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public object ActualValue
		{
			get
			{
				return DataConverter.ChangeType<string>(this._OriginalValue, GetActualType(this._OriginalType));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Validate()
		{
			if (ActualValue != null)
				return;
		}

		private System.Type GetActualType(DataType originalType)
		{
			System.Type actualType = typeof(System.String);

			switch (originalType)
			{
				case DataType.String:
					actualType = typeof(string);
					break;

				case DataType.Int:
					actualType = typeof(System.Int32);
					break;

				case DataType.Boolean:
					actualType = typeof(System.Boolean);
					break;

				case DataType.DateTime:
					actualType = typeof(System.DateTime);
					break;

				case DataType.Double:
					actualType = typeof(System.Double);
					break;

				case DataType.Float:
					actualType = typeof(System.Single);
					break;

				default:
					ExceptionHelper.TrueThrow<WfDescriptorException>(true, Resource.CanNotMapVarDataTypeToSysType, originalType);
					break;
			}

			return actualType;
		}

		#region ISerializable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("OriginalType", this._OriginalType, typeof(DataType));
			info.AddValue("OriginalValue", this._OriginalValue, typeof(string));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfVariableDescriptor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._OriginalType = (DataType)info.GetValue("OriginalType", typeof(DataType));
			this._OriginalValue = info.GetString("OriginalValue");
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WfVariableDescriptorCollection : WfDescriptorCollectionBase<WfVariableDescriptor>
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		internal WfVariableDescriptorCollection()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Add(WfVariableDescriptor item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");
			ExceptionHelper.TrueThrow<ArgumentException>(String.IsNullOrEmpty(item.Key), "VariableDescriptorKey");

			InnerAdd(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

			InnerRemove(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="variableDesp"></param>
		public void Remove(WfVariableDescriptor variableDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(variableDesp != null, "variableDesp");

			InnerRemove(variableDesp);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetValue<T>(string key, T defaultValue)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(key, "key");

			T result = defaultValue;

			WfVariableDescriptor data = this[key];

			if (data != null)
				result = (T)DataConverter.ChangeType<object>(data.ActualValue, typeof(T));

			return result;
		}

		/// <summary>
		/// 将变量的值复制到字典中
		/// </summary>
		/// <param name="dict"></param>
		public void CopyTo(IDictionary<string, object> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			dict.Clear();

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (WfVariableDescriptor variable in this)
					dict.Add(variable.Key, variable.ActualValue);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}
	}
}
