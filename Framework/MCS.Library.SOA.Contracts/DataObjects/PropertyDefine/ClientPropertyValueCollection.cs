using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
   
    [CollectionDataContract(IsReference=true)]
	public class ClientPropertyValueCollection : EditableKeyedDataObjectCollectionBase<string, ClientPropertyValue>
	{
		public T GetValue<T>(string name, T defaultValue)
		{
			name.CheckStringIsNullOrEmpty("key");

			T result = defaultValue;

			ClientPropertyValue v = this[name];

			if (v != null)
			{
				result = (T)DataConverter.ChangeType(v.StringValue, result.GetType());
			}

			return result;
		}

		public void SetValue<T>(string name, T data)
		{
			name.CheckStringIsNullOrEmpty("key");

			ClientPropertyValue v = this[name];

			(v != null).FalseThrow("不能找到名称为{0}的属性", name);

			v.StringValue = data.ToString();
		}

		protected override string GetKeyForItem(ClientPropertyValue item)
		{
			return item.Definition.Name;
		}
	}



  
}
