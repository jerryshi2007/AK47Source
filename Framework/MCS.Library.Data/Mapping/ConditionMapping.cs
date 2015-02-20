using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 根据Condition生成WhereSqlClauseBuilder的时候，对赋与条件表达式的值进行调整
	/// </summary>
	/// <param name="propertyName"></param>
	/// <param name="propertyValue"></param>
	/// <param name="ignored"></param>
	/// <returns></returns>
	public delegate object AdjustConditionValueDelegate(string propertyName, object propertyValue, ref bool ignored);

	/// <summary>
	/// 条件表达式和对象的映射关系类
	/// </summary>
	public static class ConditionMapping
	{
		private class RelativeAttributes
		{
			public ConditionMappingAttribute FieldMapping = null;
			public NoMappingAttribute NoMapping = null;
			public List<SubConditionMappingAttribute> SubClassFieldMappings = new List<SubConditionMappingAttribute>();
			public SubClassTypeAttribute SubClassType = null;
		}

		#region 公有方法
		/// <summary>
		/// 得到某个类型的条件表达式映射方式
		/// </summary>
		/// <param name="type">类型信息</param>
		/// <returns>映射方式</returns>
		public static ConditionMappingItemCollection GetMappingInfo(System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			ConditionMappingItemCollection result = null;

			if (ConditionMappingCache.Instance.TryGetValue(type, out result) == false)
			{
				result = GetMappingItemCollection(type);
				ConditionMappingCache.Instance[type] = result;
			}

			return result;
		}

		/// <summary>
		/// 根据条件对象生成WhereSqlClauseBuilder
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder(object condition)
		{
			return GetWhereSqlClauseBuilder(condition, true);
		}

		/// <summary>
		/// 根据条件对象生成WhereSqlClauseBuilder
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="acv"></param>
		/// <returns></returns>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder(object condition, AdjustConditionValueDelegate acv)
		{
			return GetWhereSqlClauseBuilder(condition, true, acv);
		}

		/// <summary>
		/// 根据条件对象生成WhereSqlClauseBuilder
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="ignoreDefaultValue">如果对象的属性值为缺省值时，不进入到WhereSqlClauseBuilder</param>
		/// <returns></returns>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder(object condition, bool ignoreDefaultValue)
		{
			return GetWhereSqlClauseBuilder(condition, ignoreDefaultValue, null);
		}

		/// <summary>
		/// 根据条件对象生成WhereSqlClauseBuilder
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="ignoreDefaultValue"></param>
		/// <param name="acv"></param>
		/// <returns></returns>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder(
				object condition,
				bool ignoreDefaultValue,
				AdjustConditionValueDelegate acv)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(condition != null, "condition");

			ConditionMappingItemCollection mapping = GetMappingInfo(condition.GetType());

			return GetWhereSqlClauseBuilderFromMapping(condition, mapping, ignoreDefaultValue, acv);
		}
		#endregion

		#region 私有方法
		private static WhereSqlClauseBuilder GetWhereSqlClauseBuilderFromMapping(
				object condition,
				ConditionMappingItemCollection mapping,
				bool ignoreDefaultValue,
				AdjustConditionValueDelegate acv)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			foreach (ConditionMappingItem item in mapping)
			{
				object data = GetValueFromObject(item, condition);

				if (data != null)
					if (ignoreDefaultValue == false || (ignoreDefaultValue == true && IsTypeDefaultValue(item, data) == false))
					{
						bool ignored = false;

						data = AdjustValueBeforeAppendToBuilder(item, data);

						if (acv != null)
							data = acv(item.PropertyName, data, ref ignored);

						if (ignored == false)
							builder.AppendItem(item.DataFieldName, data, item.Operation, item.Template, item.IsExpression);
					}
			}

			return builder;
		}

		private static object AdjustValueBeforeAppendToBuilder(ConditionMappingItem item, object data)
		{
			object result = data;

			if (data is string)
			{
				if (item.EscapeLikeString)
					result = TSqlBuilder.Instance.EscapeLikeString(data.ToString());

				if (item.Prefix.IsNotEmpty())
					result = item.Prefix + result;

				if (item.Postfix.IsNotEmpty())
					result = result + item.Postfix;
			}
			else
			{
				if (data is DateTime && (DateTime)data != DateTime.MinValue && (DateTime)data != DateTime.MaxValue)
				{
					if (item.AdjustDays != 0)
						result = ((DateTime)data).AddDays(item.AdjustDays);
				}
			}

			return result;
		}

		private static bool IsTypeDefaultValue(ConditionMappingItem item, object data)
		{
			bool result = false;

			if (data != null)
			{
				Type type = GetMemberInfoType(item.MemberInfo);

				if (type == typeof(object))
					type = data.GetType();

				if (type.IsEnum)
					result = false;
				else
				{
					if (type == typeof(string))
						result = string.IsNullOrEmpty((string)data);
					else
						result = data.Equals(TypeCreator.GetTypeDefaultValue(type));
				}
			}
			else
				result = true;

			return result;
		}

		private static Type GetMemberInfoType(MemberInfo mi)
		{
			Type result = null;

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					result = ((PropertyInfo)mi).PropertyType;
					break;
				case MemberTypes.Field:
					result = ((FieldInfo)mi).FieldType;
					break;
				default:
					ThrowInvalidMemberInfoTypeException(mi);
					break;
			}

			return result;
		}

		private static object GetValueFromObject(ConditionMappingItem item, object graph)
		{
			object data = null;

			if (string.IsNullOrEmpty(item.SubClassPropertyName))
				data = GetValueFromObjectDirectly(item, graph);
			else
			{
				if (graph != null)
				{
					MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (mi == null)
						mi = graph.GetType().GetField(item.PropertyName,
							BindingFlags.Instance | BindingFlags.Public);

					if (mi != null)
					{
						object subGraph = GetMemberValueFromObject(mi, graph);

						if (subGraph != null)
							data = GetValueFromObjectDirectly(item, subGraph);
					}
				}
			}

			return data;
		}

		private static object GetValueFromObjectDirectly(ConditionMappingItem item, object graph)
		{
			object data = GetMemberValueFromObject(item.MemberInfo, graph);

			if (data != null)
			{
				System.Type dataType = data.GetType();

				if (dataType.IsEnum)
				{
					if (item.EnumUsage == EnumUsageTypes.UseEnumValue)
						data = (int)data;
					else
						data = data.ToString();
				}
				else
				{
					if (dataType == typeof(TimeSpan))
						data = ((TimeSpan)data).TotalSeconds;
				}
			}

			return data;
		}

		private static object GetMemberValueFromObject(MemberInfo mi, object graph)
		{
			object data = null;

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					PropertyInfo pi = (PropertyInfo)mi;
					if (pi.CanRead)
						data = pi.GetValue(graph, null);
					break;
				case MemberTypes.Field:
					FieldInfo fi = (FieldInfo)mi;
					data = fi.GetValue(graph);
					break;
				default:
					ThrowInvalidMemberInfoTypeException(mi);
					break;
			}

			return data;
		}

		private static ConditionMappingItemCollection GetMappingItemCollection(System.Type type)
		{
			ConditionMappingItemCollection result = new ConditionMappingItemCollection();

			MemberInfo[] mis = GetTypeMembers(type);

			foreach (MemberInfo mi in mis)
			{
				if (mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
				{
					ConditionMappingItemCollection items = CreateMappingItems(mi);

					MergeMappingItems(result, items);
				}
			}

			return result;
		}

		private static MemberInfo[] GetTypeMembers(System.Type type)
		{
			List<MemberInfo> list = new List<MemberInfo>();

			PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

			Array.ForEach(pis, delegate(PropertyInfo pi) { list.Add(pi); });

			FieldInfo[] fis = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			Array.ForEach(fis, delegate(FieldInfo fi) { list.Add(fi); });

			return list.ToArray();
		}

		private static ConditionMappingItemCollection CreateMappingItems(MemberInfo mi)
		{
			ConditionMappingItemCollection result = null;
			bool isDoMapping = false;

			RelativeAttributes attrs = null;

			if (mi.Name != "Item" && GetRealType(mi).GetInterface("ICollection") == null)
			{
				attrs = GetRelativeAttributes(mi);

				if (attrs.NoMapping == null)
					isDoMapping = true;
			}

			if (isDoMapping == true)
			{
				if (attrs != null)
				{
					if (attrs.SubClassFieldMappings.Count > 0)
						result = GetMappingItemsBySubClass(attrs, mi);
					else
						result = GetMappingItems(attrs, mi);
				}
			}
			else
				result = new ConditionMappingItemCollection();

			return result;
		}

		private static void MergeMappingItems(ConditionMappingItemCollection dest, ConditionMappingItemCollection src)
		{
			foreach (ConditionMappingItem item in src)
				dest.Add(item);
		}

		private static RelativeAttributes GetRelativeAttributes(MemberInfo mi)
		{
			RelativeAttributes result = new RelativeAttributes();

			Attribute[] attrs = Attribute.GetCustomAttributes(mi, true);

			foreach (Attribute attr in attrs)
			{
				if (attr is NoMappingAttribute)
				{
					result.NoMapping = (NoMappingAttribute)attr;
					break;
				}
				else
					if (attr is SubConditionMappingAttribute)
						result.SubClassFieldMappings.Add((SubConditionMappingAttribute)attr);
					else
						if (attr is SubClassTypeAttribute)
							result.SubClassType = (SubClassTypeAttribute)attr;
						else
							if (attr is ConditionMappingAttribute)
								result.FieldMapping = (ConditionMappingAttribute)attr;
			}

			return result;
		}

		private static System.Type GetRealType(MemberInfo mi)
		{
			System.Type type = null;

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					type = ((PropertyInfo)mi).PropertyType;
					break;
				case MemberTypes.Field:
					type = ((FieldInfo)mi).FieldType;
					break;
				default:
					ThrowInvalidMemberInfoTypeException(mi);
					break;
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
				type = type.GetGenericArguments()[0];

			return type;
		}

		private static void ThrowInvalidMemberInfoTypeException(MemberInfo mi)
		{
			throw new InvalidOperationException(string.Format("非法的成员类型{0},{1}",
				mi.Name,
				mi.MemberType));
		}

		private static ConditionMappingItemCollection GetMappingItems(RelativeAttributes attrs, MemberInfo mi)
		{
			ConditionMappingItemCollection items = new ConditionMappingItemCollection();

			ConditionMappingItem item = new ConditionMappingItem();
			item.PropertyName = mi.Name;
			item.DataFieldName = mi.Name;

			if (attrs.FieldMapping != null)
				FillMappingItemByAttr(item, attrs.FieldMapping);

			item.MemberInfo = mi;

			items.Add(item);

			return items;
		}

		private static ConditionMappingItemCollection GetMappingItemsBySubClass(RelativeAttributes attrs, MemberInfo sourceMI)
		{
			ConditionMappingItemCollection items = new ConditionMappingItemCollection();
			System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

			MemberInfo[] mis = GetTypeMembers(subType);

			foreach (SubConditionMappingAttribute attr in attrs.SubClassFieldMappings)
			{
				MemberInfo mi = GetMemberInfoByName(attr.SubPropertyName, mis);

				if (mi != null)
				{
					ConditionMappingItem item = new ConditionMappingItem();

					item.PropertyName = sourceMI.Name;
					item.SubClassPropertyName = attr.SubPropertyName;
					item.MemberInfo = mi;

					if (attrs.SubClassType != null)
						item.SubClassTypeDescription = attrs.SubClassType.TypeDescription;

					FillMappingItemByAttr(item, attr);

					items.Add(item);
				}
			}

			return items;
		}

		private static MemberInfo GetMemberInfoByName(string name, MemberInfo[] mis)
		{
			MemberInfo result = null;

			foreach (MemberInfo mi in mis)
			{
				if (mi.Name == name)
				{
					result = mi;
					break;
				}
			}

			return result;
		}

		private static ConditionMappingItem FindItemBySubClassPropertyName(string subPropertyName, ConditionMappingItemCollection items)
		{
			ConditionMappingItem result = null;

			foreach (ConditionMappingItem item in items)
			{
				if (item.SubClassPropertyName == subPropertyName)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		private static void FillMappingItemByAttr(ConditionMappingItem item, ConditionMappingAttribute fm)
		{
			item.DataFieldName = fm.DataFieldName;
			item.Operation = fm.Operation;
			item.IsExpression = fm.IsExpression;
			item.EnumUsage = fm.EnumUsage;
			item.Prefix = fm.Prefix;
			item.Postfix = fm.Postfix;
			item.AdjustDays = fm.AdjustDays;
			item.Template = fm.Template;
			item.EscapeLikeString = fm.EscapeLikeString;
		}
		#endregion
	}
}
