using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Descriptor�Ĺ����ӿ�
	/// </summary>
	public interface IWfDescriptor
	{
		/// <summary>
		/// ������ʵ������
		/// </summary>
		IWfProcess ProcessInstance { get; }
	}

	/// <summary>
	/// ��Key��Descriptor�Ĺ����ӿ�
	/// </summary>
	public interface IWfKeyedDescriptor : IWfDescriptor
	{
		/// <summary>
		/// Descriptor��Key
		/// </summary>
		string Key
		{
			get;
		}

		/// <summary>
		/// Descriptor������
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Descriptor������
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
		/// ͬ�����Լ����е�ֵ���ڲ��ĳ�Ա���������Գ�Ա��һ����
		/// </summary>
		void SyncPropertiesToFields();
	}
}
