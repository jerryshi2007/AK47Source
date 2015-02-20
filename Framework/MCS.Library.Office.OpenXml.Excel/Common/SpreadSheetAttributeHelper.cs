using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// Excel
	/// </summary>
	public static class SpreadSheetAttributeHelper
	{
		public static TableDescription GetTableDescription(DataTable table)
		{
			return new TableDescription(table);
		}

		/// <summary>
		/// 从类型上加载TableDescription信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static TableDescription GetTableDescription<T>()
		{
			return GetTableDescription(typeof(T));
		}

		/// <summary>
		/// 从类型上加载TableDescription信息
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TableDescription GetTableDescription(Type type)
		{
			type.NullCheck("type");

			return TableDescriptionCacheQueue.Instance.GetOrAddNewValue(type, (cache, key) =>
			{
				TableDescription result = GenerateTableDescription(type);

				cache.Add(key, result);

				return result;
			});
		}

		#region 沈峥
		private static TableDescription GenerateTableDescription(Type type)
		{
			TableDescription tableDesp = new TableDescription(type);

			Dictionary<string, PropertyInfo> propertiesDict = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(type);

			List<PropertyInfo> properties = RemoveNotTableColumnPropeties(propertiesDict);

			tableDesp.AllColumns.InitFromProperties(properties);

			return tableDesp;
		}

		private static List<PropertyInfo> RemoveNotTableColumnPropeties(Dictionary<string, PropertyInfo> propertiesDict)
		{
			List<PropertyInfo> result = new List<PropertyInfo>();

			foreach (KeyValuePair<string, PropertyInfo> kp in propertiesDict)
			{
				if (AttributeHelper.GetCustomAttribute<NotTableColumnAttribute>(kp.Value) == null)
					result.Add(kp.Value);
			}

			return result;
		}

		#endregion

		#region 晏德智
		public static bool TryParse<T>(PropertyInfo propertyinfo, out T converValue) where T : Attribute
		{
			bool result = false;
			Type propertyType = typeof(T);
			if (propertyinfo.IsDefined(propertyType, false))
			{
				result = true;
				converValue = (T)propertyinfo.GetCustomAttributes(propertyType, true)[0];
			}
			else
			{
				converValue = default(T);
			}

			return result;
		}
		#endregion 晏德智
	}

	public static class SpreadSheetExcportHelper
	{
		/// <summary>
		/// 根据数据设置对象属性值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="customLog"></param>
		/// <param name="objItem"></param>
		/// <returns></returns>
		public static bool SetPropertiesValue<T>(ExcportRowContext context, StringBuilder customLog, T objItem)
		{
			bool result = false;
			Dictionary<string, PropertyInfo> properties = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(typeof(T));
			foreach (ExportCellDescription cellDesp in context.PropertyDescriptions)
			{
				if (properties.ContainsKey(cellDesp.PropertyName))
				{
					PropertyInfo propertyInfo = properties[cellDesp.PropertyName];
					try
					{
						if (cellDesp.Value != null)
							propertyInfo.SetValue(objItem, DataConverter.ChangeType(cellDesp.Value, propertyInfo.PropertyType), null);
					}
					catch (Exception ex)
					{
						customLog.AppendFormat("{0}单元格值:{1},不能转换到指定对象{2}属性！", cellDesp.Address, cellDesp.Value.ToString(), cellDesp.PropertyName);
						customLog.AppendFormat("错误信息{0}", ex.ToString());

						result = false;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 设置指定对象的一个值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objItem"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal static void SetPropertyValue<T>(T objItem, string propertyName, object value)
		{
			if (value != null)
			{
				Dictionary<string, PropertyInfo> properties = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(typeof(T));
				if (properties.ContainsKey(propertyName))
				{
					PropertyInfo propertyInfo = properties[propertyName];
					propertyInfo.SetValue(objItem, DataConverter.ChangeType(value, propertyInfo.PropertyType), null);
				}
			}
		}

		/*
		private void SetPropertyValue<T>(T data, PropertyInfo properytInfo, CellBase tbCell)
		{
			if (properytInfo.PropertyType.IsEnum)
				properytInfo.SetValue(data, Enum.ToObject(properytInfo.PropertyType, tbCell.Value), null);
			else
			{
				//TODO：考虑DataConverter和转换逻辑之间的复用性
				//DataConverter.ChangeType(tbCell.Value, properytInfo.PropertyType);
				if (properytInfo.PropertyType == typeof(string))
					properytInfo.SetValue(data, tbCell.GetTypedValue<string>(), null);
				else
					if (properytInfo.PropertyType == typeof(double))
						properytInfo.SetValue(data, tbCell.GetTypedValue<double>(), null);
					else if (properytInfo.PropertyType == typeof(decimal))
						properytInfo.SetValue(data, tbCell.GetTypedValue<decimal>(), null);
					else if (properytInfo.PropertyType == typeof(DateTime))
						properytInfo.SetValue(data, tbCell.GetTypedValue<DateTime>(), null);
					else if (properytInfo.PropertyType == typeof(TimeSpan))
						properytInfo.SetValue(data, tbCell.GetTypedValue<TimeSpan>(), null);
					else
						properytInfo.SetValue(data, tbCell.Value, null);
			}
		} */

	}
}
