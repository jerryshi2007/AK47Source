using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public enum WfSvcOperationParameterType
	{
		Int,
		String,
		DateTime,
		RuntimeParameter,
		Boolean
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceOperationParameter : ISimpleXmlSerializer
	{
		public WfServiceOperationParameter()
		{
		}

		/// <summary>
		/// 初始化服务方法调用时的参数，默认是字符串类型的
		/// </summary>
		/// <param name="name">参数名称</param>
		/// <param name="value"></param>
		public WfServiceOperationParameter(string name, string value)
		{
			name.CheckStringIsNullOrEmpty("name");

			this.Name = name;
			this.Type = WfSvcOperationParameterType.String;
			this.Value = value;
		}

		/// <summary>
		/// 初始化服务方法调用时的参数
		/// </summary>
		/// <param name="name">参数名称</param>
		/// <param name="type">参数类型</param>
		/// <param name="value">参数值</param>
		public WfServiceOperationParameter(string name, WfSvcOperationParameterType type, object value)
		{
			name.CheckStringIsNullOrEmpty("name");

			this.Name = name;
			this.Type = type;
			this.Value = value;
		}

		public string Name { get; set; }
		public WfSvcOperationParameterType Type { get; set; }

		/// <summary>
		/// 当参数类型是复杂类型时，参数值是流程上下文参数名称
		/// </summary>
		public object Value { get; set; }

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("name", this.Name);

			if (this.Value != null)
			{
				if (this.Value is ISimpleXmlSerializer)
					((ISimpleXmlSerializer)this.Value).ToXElement(element, string.Empty);
				else
					SetXElementStringValue(element, "value", this.Value);
			}
		}

		#endregion

		private static void SetXElementStringValue(XElement element, string key, object data)
		{
			try
			{
				element.SetAttributeValue(key, DataConverter.ChangeType(data, typeof(string)));
			}
			catch (System.Exception)
			{
			}
		}
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceOperationParameterCollection : EditableDataObjectCollectionBase<WfServiceOperationParameter>, ISimpleXmlSerializer
	{
		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			foreach (WfServiceOperationParameter param in this)
			{
				XElement paramElement = element.AddChildElement("Param", string.Empty);

				((ISimpleXmlSerializer)param).ToXElement(paramElement, string.Empty);
			}
		}

		#endregion
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceOperationDefinition : ISimpleXmlSerializer
	{
		public WfServiceOperationDefinition()
		{

		}

		public WfServiceOperationDefinition(string addressKey,
			string operationName,
			WfServiceOperationParameterCollection parameters,
			string xmlStoreParaName)
		{
			if (WfGlobalParameters.Default.ServiceAddressDefs[addressKey] == null)
				throw new ArgumentException(string.Format("无法找到Key为{0}的服务地址定义", addressKey));

			this._AddressDef = WfGlobalParameters.Default.ServiceAddressDefs[addressKey];
			this.OperationName = operationName;
			this.Params = parameters;
			this.RtnXmlStoreParamName = xmlStoreParaName;
		}

		public WfServiceOperationDefinition(WfServiceAddressDefinition address,
			string operationName)
		{
			this._AddressDef = address;
			this.OperationName = operationName;
		}

		public WfServiceOperationDefinition(WfServiceAddressDefinition address,
			string operationName, WfServiceOperationParameterCollection parameters,
			string xmlStoreParaName)
		{
			this._AddressDef = address;
			this.OperationName = operationName;
			this.Params = parameters;
			this.RtnXmlStoreParamName = xmlStoreParaName;
		}

		public string AddressKey { get; set; }	//兼容2012-02-03之前保存的数据，故保留该属性

		private WfServiceAddressDefinition _AddressDef = null;
		public WfServiceAddressDefinition AddressDef
		{
			get
			{
				// 如果全局地址表包含Key，那么就是用此Key去全局地址表取地址，否则使用默认的。
				if (string.IsNullOrEmpty(this.AddressKey) == false)
				{
					this._AddressDef = WfGlobalParameters.Default.ServiceAddressDefs[this.AddressKey];
				}
				else
				{
					if (this._AddressDef != null && string.IsNullOrEmpty(this._AddressDef.Key) == false && WfGlobalParameters.Default.ServiceAddressDefs.ContainsKey(this._AddressDef.Key))
					{
						return WfGlobalParameters.Default.ServiceAddressDefs[this._AddressDef.Key];
					}
				}

				return this._AddressDef;
			}
			internal set
			{
				this._AddressDef = value;
			}
		}

		/// <summary>
		/// 是否在流程持久化时调用。
		/// </summary>
		public bool InvokeWhenPersist
		{
			get;
			set;
		}

		public string OperationName
		{
			get;
			set;
		}

		private double _TimeOut = 0;

		/// <summary>
		/// 调用服务超时间 
		/// </summary>
		//2012-11-28
		public double TimeOut
		{
			get
			{
				return this._TimeOut;
			}
			set
			{
				this._TimeOut = value;
			}
		}

		private WfServiceOperationParameterCollection _Params;
		public WfServiceOperationParameterCollection Params
		{
			get
			{
				if (_Params == null)
					_Params = new WfServiceOperationParameterCollection();

				return _Params;
			}
			set
			{
				_Params = value;
			}
		}

		/// <summary>
		/// 服务返回的xml存放在流程中的参数名
		/// </summary>
		public string RtnXmlStoreParamName { get; set; }

		public string Key { get; set; }

		public WfServiceOperationDefinition Clone()
		{
			return (WfServiceOperationDefinition)SerializationHelper.CloneObject(this);
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("key", this.Key);
			element.SetAttributeValue("rtnXmlStoreParamName", this.RtnXmlStoreParamName);

			if (this._Params != null)
				((ISimpleXmlSerializer)this._Params).ToXElement(element, "Params");

			if (this.AddressDef != null)
				((ISimpleXmlSerializer)this.AddressDef).ToXElement(element, "AddressDef");
		}

		#endregion
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceOperationDefinitionCollection : EditableDataObjectCollectionBase<WfServiceOperationDefinition>, ISimpleXmlSerializer
	{
		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			foreach (WfServiceOperationDefinition operation in this)
				((ISimpleXmlSerializer)operation).ToXElement(element, "Operation");
		}

		#endregion

		/// <summary>
		/// 得到在持久化时需要调用的服务
		/// </summary>
		/// <returns></returns>
		public WfServiceOperationDefinitionCollection GetServiceOperationsWhenPersist()
		{
			WfServiceOperationDefinitionCollection result = new WfServiceOperationDefinitionCollection();

			foreach (WfServiceOperationDefinition serviceOpDef in this)
			{
				if (serviceOpDef.InvokeWhenPersist)
					result.Add(serviceOpDef);
			}

			return result;
		}

		/// <summary>
		/// 得到在持久化之前需要调用的服务
		/// </summary>
		/// <returns></returns>
		public WfServiceOperationDefinitionCollection GetServiceOperationsBeforePersist()
		{
			WfServiceOperationDefinitionCollection result = new WfServiceOperationDefinitionCollection();

			foreach (WfServiceOperationDefinition serviceOpDef in this)
			{
				if (serviceOpDef.InvokeWhenPersist == false)
					result.Add(serviceOpDef);
			}

			return result;
		}

		public void SyncPropertiesToFields(PropertyValue property)
		{
			if (property != null)
			{
				this.Clear();

				if (property.StringValue.IsNotEmpty())
				{
					IEnumerable<WfServiceOperationDefinition> deserializedData = (IEnumerable<WfServiceOperationDefinition>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

					this.CopyFrom(deserializedData);
				}
			}
		}
	}
}
