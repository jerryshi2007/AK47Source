using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Descriptor的公共接口
	/// </summary>
	public interface IWfDescriptor
	{
		/// <summary>
		/// 所属的实例对象
		/// </summary>
		IWfProcess ProcessInstance { get; }
	}

	/// <summary>
	/// 带Key的Descriptor的公共接口
	/// </summary>
	public interface IWfKeyedDescriptor : IWfDescriptor
	{
		/// <summary>
		/// Descriptor的Key
		/// </summary>
		string Key
		{
			get;
		}

		/// <summary>
		/// Descriptor的名称
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Descriptor的描述
		/// </summary>
		string Description
		{
			get;
		}

		bool Enabled
		{
			get;
		}

		PropertyValueCollection Properties
		{
			get;
		}

		/// <summary>
		/// 同步属性集合中的值到内部的成员，保持属性成员的一致性
		/// </summary>
		void SyncPropertiesToFields();
	}
}
