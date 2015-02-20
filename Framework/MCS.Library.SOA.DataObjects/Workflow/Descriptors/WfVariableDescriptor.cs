using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 
	/// </summary>
	public enum DataType
	{
		[EnumItemDescription("字符串")]
		[DataTypeDescription("System.String")]
		String,

		[EnumItemDescription("整型")]
		[DataTypeDescription("System.Int32")]
		Int,

		[EnumItemDescription("双精度浮点型")]
		[DataTypeDescription("System.Double")]
		Double,

		[EnumItemDescription("单精度浮点型")]
		[DataTypeDescription("System.Single")]
		Float,

		[EnumItemDescription("布尔型")]
		[DataTypeDescription("System.Boolean")]
		Boolean,

		[EnumItemDescription("日期型")]
		[DataTypeDescription("System.DateTime")]
		DateTime,

		[EnumItemDescription("字符串数组")]
		[DataTypeDescription("System.String[]")]
		StringArray,

		[EnumItemDescription("整型数组")]
		[DataTypeDescription("System.Int32[]")]
		IntArray,

		//JSON序列化可能有问题
		//[EnumItemDescription("双精度浮点型数组")]
		//[DataTypeDescription("System.Double[]")]
		//DoubleArray,

		//[EnumItemDescription("单精度浮点型数组")]
		//[DataTypeDescription("System.Single[]")]
		//FloatArray,

		[EnumItemDescription("布尔型数组")]
		[DataTypeDescription("System.Boolean[]")]
		BooleanArray
	}

	/// <summary>
	/// 变量列表
	/// </summary>
	/// <remarks></remarks>
	[Serializable]
	[XElementSerializable]
	public class WfVariableDescriptor : WfKeyedDescriptorBase
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
				Type type = GetActualType(this._OriginalType);
				object result = null;

				if (type.IsPrimitive == false && type.IsArray)
					result = JSONSerializerExecute.DeserializeObject(this._OriginalValue, type);
				else
					result = DataConverter.ChangeType<string>(this._OriginalValue, GetActualType(this._OriginalType));

				return result;
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

		public WfVariableDescriptor Clone()
		{
			WfVariableDescriptor variable = new WfVariableDescriptor();

			this.CloneProperties(variable);

			variable._OriginalType = this._OriginalType;
			variable._OriginalValue = this._OriginalValue;

			return variable;
		}

		private System.Type GetActualType(DataType originalType)
		{
			System.Type actualType = typeof(System.String);

			DataTypeDescriptionAttribute typeDesp = AttributeHelper.GetCustomAttribute<DataTypeDescriptionAttribute>(originalType.GetType().GetField(originalType.ToString()));

			if (typeDesp != null && typeDesp.TypeDescription.IsNotEmpty())
				actualType = TypeCreator.GetTypeInfo(typeDesp.TypeDescription);

			return actualType;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfVariableDescriptorCollection : WfKeyedDescriptorCollectionBase<WfVariableDescriptor>
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		internal WfVariableDescriptorCollection(IWfDescriptor owner)
			: base(owner)
		{
		}

		public WfVariableDescriptorCollection()
			: base(null)
		{
		}

		protected WfVariableDescriptorCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
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
		/// 设置变量的值，如果不存在，则增加一个变量，否则替换掉
		/// </summary>
		/// <param name="key"></param>
		/// <param name="originalValue"></param>
		public void SetValue(string key, string originalValue)
		{
			SetValue(key, originalValue, DataType.String);
		}

		/// <summary>
		/// 设置变量的值，如果不存在，则增加一个变量，否则替换掉
		/// </summary>
		/// <param name="key"></param>
		/// <param name="originalValue"></param>
		/// <param name="dataType"></param>
		public void SetValue(string key, string originalValue, DataType dataType)
		{
			if (this.ContainsKey(key))
				this.Remove(v => v.Key == key);

			this.Add(new WfVariableDescriptor(key, originalValue, dataType));
		}

		/// <summary>
		/// 同步非保留字的变量
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reservedVariableNames"></param>
		public void SyncPropertiesToFields(PropertyValue property, string[] reservedVariableNames)
		{
			WfVariableDescriptorCollection tempVariables = new WfVariableDescriptorCollection();
			
			tempVariables.SyncPropertiesToFields(property);

			this.Remove(v => reservedVariableNames.NotExists(s => s == v.Key));

			foreach (WfVariableDescriptor v in tempVariables)
			{
				if (reservedVariableNames.NotExists(s => s == v.Key))
					this.Add(v);
			}
		}
	}
}
