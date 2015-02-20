using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 属性值的存取器
	/// </summary>
	public interface IPropertyValueAccessor
	{
		string StringValue
		{
			get;
			set;
		}

		PropertyDefine Definition
		{
			get;
			set;
		}
	}

	public interface IDefinitionProperties<T> where T : IPropertyValueAccessor
	{
		SerializableEditableKeyedDataObjectCollectionBase<string, T> Properties { get; }
	}

}
