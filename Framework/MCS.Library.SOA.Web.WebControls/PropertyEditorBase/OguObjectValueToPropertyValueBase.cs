using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	public abstract class OguObjectValueToPropertyValueConverterBase<T> : IObjectValueToPropertyValue
	{
		public OguObjectValueToPropertyValueConverterBase()
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
		}

		#region IObjectValueToPropertyValue Members

		public void ConvertToPropertyValue(Type objectValueType, object objectValue, PropertyValue pv, object target)
		{
			CheckObjectValueType(objectValueType, objectValue);

			pv.Definition.EditorKey = "OUUserInputPropertyEditor";
			pv.Definition.EditorParams = JSONSerializerExecute.Serialize(CreateEditorParams(objectValueType, objectValue));

			if (objectValue != null)
			{
				if (objectValue is IEnumerable == false)
					objectValue = OguBase.CreateWrapperObject((IOguObject)objectValue);

				pv.StringValue = GetPropertyValue(objectValueType, objectValue);
			}
		}

		public object PropertyValueToObjectValue(PropertyValue pv, Type objectValueType, object originalObjectValue, object target)
		{
			return GetObjectValue(pv);
		}

		#endregion

		/// <summary>
		/// 检查对象的数据类型
		/// </summary>
		/// <param name="objectValue"></param>
		protected virtual void CheckObjectValueType(Type objectValueType, object objectValue)
		{
			(objectValue is T).FalseThrow("数据类型必须是{0}", typeof(T).FullName);
		}

		/// <summary>
		/// 转换为属性的值
		/// </summary>
		/// <param name="objectValueType"></param>
		/// <param name="objectValue"></param>
		/// <returns></returns>
		protected virtual string GetPropertyValue(Type objectValueType, object objectValue)
		{
			string result = string.Empty;

			if (objectValue is IEnumerable)
			{
				result = JSONSerializerExecute.Serialize(objectValue);
			}
			else
			{
				T[] array = new T[1];
				array[0] = (T)objectValue;

				result = JSONSerializerExecute.Serialize(array);
			}

			return result;
		}

		protected virtual T GetObjectValue(PropertyValue pv)
		{
			T result = default(T);

			if (pv.StringValue.IsNotEmpty())
				result = JSONSerializerExecute.Deserialize<T>(pv.StringValue);

			return result;
		}

		internal virtual OUUserInputEditorParams CreateEditorParams(Type objectValueType, object objectValue)
		{
			OUUserInputEditorParams result = new OUUserInputEditorParams();

			if (objectValue is IEnumerable)
				result.multiSelect = true;

			return result;
		}
	}
}
