#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	ORMapping.cs
// Remark	：	提供一些对象和数据字段进行转换的静态方法
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Properties;
using MCS.Library.Data.DataObjects;
using MCS.Library.Caching;
using MCS.Library.Security;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 映射过程中，事件的参数
	/// </summary>
	public sealed class MappingEventArgs
	{
		/// <summary>
		/// 数据对象的属性名
		/// </summary>
		public string PropertyName
		{
			get;
			internal set;
		}

		/// <summary>
		/// 数据源的字段名称
		/// </summary>
		public string DataFieldName
		{
			get;
			internal set;
		}

		/// <summary>
		/// 映射时的数据对象
		/// </summary>
		public object Graph
		{
			get;
			internal set;
		}
	}

	/// <summary>
	/// 数据源映射到对象时，如果需要创建子对象，则触发此对象
	/// </summary>
	/// <param name="dataSource">数据源</param>
	/// <param name="args"></param>
	/// <param name="useDefaultObject">是否使用缺省的构造对象</param>
	/// <returns></returns>
	public delegate object CreateSubObjectDelegate(object dataSource, MappingEventArgs args, ref bool useDefaultObject);

	/// <summary>
	/// 映射Data到对象时的委托集合定义
	/// </summary>
	public class DataToObjectDeligations
	{
		/// <summary>
		/// 
		/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event CreateSubObjectDelegate CreateSubObject;

		internal object OnCreateSubObjectDelegate(object dataSource, MappingEventArgs args, ref bool useDefaultObject)
		{
			object result = null;

			if (CreateSubObject != null)
				result = CreateSubObject(dataSource, args, ref useDefaultObject);

			return result;
		}
	}

	/// <summary>
	/// 进行ORMapping的功能类
	/// </summary>
	/// <remarks>
	/// 提供一些对象和数据字段进行转换的静态方法
	/// </remarks>
	public static partial class ORMapping
	{
		private class RelativeAttributes
		{
			public ORFieldMappingAttribute FieldMapping = null;
			public SqlBehaviorAttribute SqlBehavior = null;
			public NoMappingAttribute NoMapping = null;
			public List<SubClassORFieldMappingAttribute> SubClassFieldMappings = new List<SubClassORFieldMappingAttribute>();
			public List<SubClassSqlBehaviorAttribute> SubClassFieldSqlBehaviors = new List<SubClassSqlBehaviorAttribute>();
			public SubClassTypeAttribute SubClassType = null;
			public PropertyEncryptionAttribute PropertyEncryption = null;
			public List<SubClassPropertyEncryptionAttribute> SubClassPropertyEncryptions = new List<SubClassPropertyEncryptionAttribute>();
		}

		private delegate void DoSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph);

		#region 私有方法
		private static object ConvertData(ORMappingItem item, object data)
		{
			try
			{
				System.Type realType = GetRealType(item.MemberInfo);

				return DataConverter.ChangeType(data, realType);
			}
			catch (System.Exception ex)
			{
				throw new SystemSupportException(
					string.Format(Resource.ConvertDataFieldToPropertyError,
						item.DataFieldName, item.PropertyName, ex.Message),
					ex
					);
			}
		}

		private static bool Convertible(System.Type targetType, object data)
		{
			bool result = true;

			if (data == null && targetType.IsValueType)
				result = false;
			else
			{
				if (data == DBNull.Value)
				{
					if (targetType != typeof(DBNull) && targetType != typeof(string))
						result = false;
				}
			}

			return result;
		}

		private static void FillSqlClauseBuilder<T>(
				SqlClauseBuilderIUW builder,
				T graph,
				ORMappingItemCollection mapping,
				ClauseBindingFlags bindingFlags,
				DoSqlClauseBuilder<T> builderDelegate,
				params string[] ignoreProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			using (ORMappingContext context = ORMappingContext.GetContext())
			{
				foreach (ORMappingItem item in mapping)
				{
					if (Array.Exists<string>(ignoreProperties, target => (string.Compare(target, item.PropertyName, true) == 0)
												) == false)
					{
						if ((item.BindingFlags & bindingFlags) != ClauseBindingFlags.None)
							builderDelegate(builder, item, graph);
					}
				}
			}

            builder.AppendTenantCode(typeof(T));
		}

		private static void DoInsertUpdateSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			if (item.IsIdentity == false)
			{
				object data = GetValueFromObject(item, graph);

				if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) &&
						string.IsNullOrEmpty(item.DefaultExpression) == false)
					builder.AppendItem(item.DataFieldName, item.DefaultExpression, SqlClauseBuilderBase.EqualTo, true);
				else
					builder.AppendItem(item.DataFieldName, FormatValue(data, item));
			}
		}

		private static void DoWhereSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			object data = GetValueFromObject(item, graph);

			if ((data == null || data == DBNull.Value))
				builder.AppendItem(item.DataFieldName, data, SqlClauseBuilderBase.Is);
			else
				builder.AppendItem(item.DataFieldName, FormatValue(data, item));
		}

		private static void DoWhereSqlClauseBuilderByPrimaryKey<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			if (item.PrimaryKey)
			{
				object data = GetValueFromObject(item, graph);

				if ((data == null || data == DBNull.Value))
					builder.AppendItem(item.DataFieldName, data, SqlClauseBuilderBase.Is);
				else
					builder.AppendItem(item.DataFieldName, data);
			}
		}

		private static void SetValueToObject(ORMappingItem item, object graph, object data, object row, DataToObjectDeligations dod)
		{
			if (string.IsNullOrEmpty(item.SubClassPropertyName))
				SetMemberValueToObject(item.MemberInfo, graph, data);
			else
			{
				if (graph != null)
				{
					MemberInfo mi = TypePropertiesWithNonPublicCacheQueue.Instance.GetPropertyInfoDirectly(graph.GetType(), item.PropertyName);

					if (mi == null)
						mi = graph.GetType().GetField(item.PropertyName,
							BindingFlags.Instance | BindingFlags.Public);

					if (mi != null)
					{
						object subGraph = GetMemberValueFromObject(mi, graph);

						if (subGraph == null)
						{
							bool useDefaultObject = true;

							if (dod != null)
							{
								MappingEventArgs args = new MappingEventArgs();

								args.DataFieldName = item.DataFieldName;
								args.PropertyName = item.PropertyName;
								args.Graph = graph;

								subGraph = dod.OnCreateSubObjectDelegate(row, args, ref useDefaultObject);
							}

							if (useDefaultObject)
							{
								if (string.IsNullOrEmpty(item.SubClassTypeDescription) == false)
									subGraph = TypeCreator.CreateInstance(item.SubClassTypeDescription);
								else
									subGraph = Activator.CreateInstance(GetRealType(mi), true);
							}

							SetMemberValueToObject(item.MemberInfo, subGraph, data);
							SetMemberValueToObject(mi, graph, subGraph);
						}
						else
							SetMemberValueToObject(item.MemberInfo, subGraph, data);
					}
				}
			}
		}

		private static void SetMemberValueToObject(MemberInfo mi, object graph, object data)
		{
			data = DecorateDate(data);

			IMemberAccessor accessor = graph as IMemberAccessor;

			if (accessor != null)
			{
				accessor.SetValue(graph, mi.Name, data);
			}
			else
			{
				switch (mi.MemberType)
				{
					case MemberTypes.Property:
						PropertyInfo pi = (PropertyInfo)mi;
						if (pi.CanWrite)
							pi.SetValue(graph, data, null);
						break;
					case MemberTypes.Field:
						FieldInfo fi = (FieldInfo)mi;
						fi.SetValue(graph, data);
						break;
					default:
						ThrowInvalidMemberInfoTypeException(mi);
						break;
				}
			}
		}

		/// <summary>
		/// 对数据进行最后的修饰，例如对日期类型的属性加工
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static object DecorateDate(object data)
		{
			object result = data;

			if (data is DateTime)
			{
				DateTime dt = (DateTime)data;

				if (dt.Kind == DateTimeKind.Unspecified)
					result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
			}

			return result;
		}

		private static object GetValueFromObject(ORMappingItem item, object graph)
		{
			object data = null;

			if (string.IsNullOrEmpty(item.SubClassPropertyName))
			{
				data = GetValueFromObjectDirectly(item, graph);

				if (item.EncryptProperty)
					data = EncryptPropertyValue(item, data);
			}
			else
			{
				if (graph != null)
				{
					MemberInfo mi = TypePropertiesWithNonPublicCacheQueue.Instance.GetPropertyInfoDirectly(graph.GetType(), item.PropertyName);

					if (mi == null)
						mi = graph.GetType().GetField(item.PropertyName,
							BindingFlags.Instance | BindingFlags.Public);

					if (mi != null)
					{
						object subGraph = GetMemberValueFromObject(mi, graph);

						if (subGraph != null)
						{
							data = GetValueFromObjectDirectly(item, subGraph);

							if (data != null && item.EncryptProperty)
								data = EncryptPropertyValue(item, data);
						}
					}
				}
			}

			return data;
		}

		/// <summary>
		/// 加密数据
		/// </summary>
		/// <param name="item"></param>
		/// <param name="originalData"></param>
		/// <returns></returns>
		private static object EncryptPropertyValue(ORMappingItem item, object originalData)
		{
			object result = originalData;

			if (originalData != null && originalData != DBNull.Value)
			{
				if (originalData is string == false || (string)originalData != string.Empty)
				{
					ISymmetricEncryption encryptor = ORMappingItemEncryptionHelper.GetEncryptor(item.EncryptorName);
					result = encryptor.EncryptString(originalData.ToString()).ToBase16String();
				}
			}

			return result;
		}

		private static object DecryptPropertyValue(ORMappingItem item, object originalData)
		{
			object result = originalData;

			if (originalData is string)
			{
				string stringValue = (string)originalData;

				if (stringValue.IsNotEmpty())
				{
					try
					{
						ISymmetricEncryption encryptor = ORMappingItemEncryptionHelper.GetEncryptor(item.EncryptorName);
						result = encryptor.DecryptString(stringValue.ToBase16Bytes());
					}
					catch (System.FormatException)
					{
					}
				}
			}

			return result;
		}

		private static object GetValueFromObjectDirectly(ORMappingItem item, object graph)
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
					if (dataType == typeof(TimeSpan))
						data = ((TimeSpan)data).TotalSeconds;
			}

			return data;
		}

		private static object GetMemberValueFromObject(MemberInfo mi, object graph)
		{
			try
			{
				object data = null;

				IMemberAccessor accessor = graph as IMemberAccessor;

				if (accessor != null)
				{
					data = accessor.GetValue(graph, mi.Name);
				}
				else
				{
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
				}

				return data;
			}
			catch (System.Exception ex)
			{
				System.Exception realEx = ExceptionHelper.GetRealException(ex);

				throw new ApplicationException(string.Format("读取属性{0}值的时候出错，{1}", mi.Name, realEx.Message));
			}
		}

		private static ORMappingItemCollection GetMappingItemCollection(System.Type type)
		{
			ORMappingItemCollection result = new ORMappingItemCollection();

			ORTableMappingAttribute tableAttr =
				(ORTableMappingAttribute)ORTableMappingAttribute.GetCustomAttribute(type, typeof(ORTableMappingAttribute), true);

			if (tableAttr != null)
				result.TableName = tableAttr.TableName;
			else
				result.TableName = type.Name;

			MemberInfo[] mis = GetTypeMembers(type);

			foreach (MemberInfo mi in mis)
			{
				if (mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
				{
					ORMappingItemCollection items = CreateMappingItems(mi);

					MergeMappingItems(result, items);
				}
			}

			return result;
		}

		private static void MergeMappingItems(ORMappingItemCollection dest, ORMappingItemCollection src)
		{
			foreach (ORMappingItem item in src)
				if (dest.ContainsKey(item.DataFieldName) == false)
					dest.Add(item);
		}

		private static ORMappingItemCollection CreateMappingItems(MemberInfo mi)
		{
			ORMappingItemCollection result = null;
			bool isDoMapping = false;

			RelativeAttributes attrs = null;

			Type realType = GetRealType(mi);

			//不处理除byte[]之外的集合类
			if (mi.Name != "Item" && (realType == typeof(byte[]) || realType.GetInterface("ICollection") == null))
			{
				attrs = GetRelativeAttributes(mi);

				if (attrs.NoMapping == null)
					isDoMapping = true;
			}

			if (isDoMapping == true)
			{
				if (attrs != null)
				{
					if (attrs.SubClassFieldMappings.Count > 0 || attrs.SubClassFieldSqlBehaviors.Count > 0)
						result = GetMappingItemsBySubClass(attrs, mi);
					else
						result = GetMappingItems(attrs, mi);
				}
			}
			else
				result = new ORMappingItemCollection();

			return result;
		}

		private static ORMappingItemCollection GetMappingItems(RelativeAttributes attrs, MemberInfo mi)
		{
			ORMappingItemCollection items = new ORMappingItemCollection();

			ORMappingItem item = new ORMappingItem();
			item.PropertyName = mi.Name;
			item.DataFieldName = mi.Name;

			if (attrs.FieldMapping != null)
				FillMappingItemByAttr(item, attrs.FieldMapping);

			if (attrs.SqlBehavior != null)
				FillMappingItemByBehaviorAttr(item, attrs.SqlBehavior);

			if (attrs.PropertyEncryption != null)
				FillMappingItemByEncryptionAttr(item, attrs.PropertyEncryption);

			item.MemberInfo = mi;
			item.DeclaringType = mi.DeclaringType;

			items.Add(item);

			return items;
		}

		private static ORMappingItemCollection GetMappingItemsBySubClass(RelativeAttributes attrs, MemberInfo sourceMI)
		{
			ORMappingItemCollection items = new ORMappingItemCollection();
			System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

			MemberInfo[] mis = GetTypeMembers(subType);

			foreach (SubClassORFieldMappingAttribute attr in attrs.SubClassFieldMappings)
			{
				MemberInfo mi = GetMemberInfoByName(attr.SubPropertyName, mis);

				if (mi != null)
				{
					if (items.ContainsKey(attr.DataFieldName) == false)
					{
						ORMappingItem item = new ORMappingItem();

						item.PropertyName = sourceMI.Name;
						item.SubClassPropertyName = attr.SubPropertyName;
						item.MemberInfo = mi;
						item.DeclaringType = sourceMI.DeclaringType;

						if (attrs.SubClassType != null)
							item.SubClassTypeDescription = attrs.SubClassType.TypeDescription;

						FillMappingItemByAttr(item, attr);

						items.Add(item);
					}
				}
			}

			foreach (SubClassSqlBehaviorAttribute attr in attrs.SubClassFieldSqlBehaviors)
			{
				ORMappingItem item = FindItemBySubClassPropertyName(attr.SubPropertyName, items);

				if (item != null)
					FillMappingItemByBehaviorAttr(item, attr);
			}

			foreach (SubClassPropertyEncryptionAttribute attr in attrs.SubClassPropertyEncryptions)
			{
				ORMappingItem item = FindItemBySubClassPropertyName(attr.SubPropertyName, items);

				if (item != null)
					FillMappingItemByEncryptionAttr(item, attr);
			}

			return items;
		}

		/// <summary>
		/// 得到属性路径中最低级别的MemberInfo
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attrs"></param>
		/// <param name="sourceMI"></param>
		/// <returns></returns>
		private static MemberInfo GetLeaveMemberInfo(string propertyName, RelativeAttributes attrs, MemberInfo sourceMI)
		{
			MemberInfo result = null;

			string[] propertyParts = propertyName.Split('.');

			for (int i = 0; i < propertyParts.Length; i++)
			{
				System.Type subType = null;

				if (i == propertyParts.Length - 1)
					subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);
				else
					subType = GetRealType(sourceMI);

				MemberInfo[] mis = GetTypeMembers(subType);

				sourceMI = GetMemberInfoByName(propertyParts[i], mis);

				if (sourceMI == null)
					break;
			}

			result = sourceMI;

			return result;
		}

		internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, MemberInfo sourceMI)
		{
			RelativeAttributes attrs = GetRelativeAttributes(sourceMI);

			System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

			return GetSubClassMemberInfoByName(subClassPropertyName, subType);
		}

		internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, System.Type subType)
		{
			MemberInfo[] mis = GetTypeMembers(subType);

			return GetMemberInfoByName(subClassPropertyName, mis);
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

		private static object FormatValue(object dataValue, ORMappingItem item)
		{
			object result = dataValue;

			if (item.Format.IsNotEmpty())
				result = string.Format(item.Format, dataValue);

			return result;
		}

		private static void FillMappingItemByBehaviorAttr(ORMappingItem item, SqlBehaviorAttribute sba)
		{
			item.BindingFlags = sba.BindingFlags;
			item.DefaultExpression = sba.DefaultExpression;
			item.EnumUsage = sba.EnumUsage;
		}

		private static void FillMappingItemByEncryptionAttr(ORMappingItem item, PropertyEncryptionAttribute pea)
		{
			item.EncryptProperty = true;
			item.EncryptorName = pea.EncryptorName;
		}

		private static ORMappingItem FindItemBySubClassPropertyName(string subPropertyName, ORMappingItemCollection items)
		{
			ORMappingItem result = null;

			foreach (ORMappingItem item in items)
			{
				if (item.SubClassPropertyName == subPropertyName)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		private static void FillMappingItemByAttr(ORMappingItem item, ORFieldMappingAttribute fm)
		{
			item.DataFieldName = fm.DataFieldName;
			item.IsIdentity = fm.IsIdentity;
			item.IsNullable = fm.IsNullable;
			item.Length = fm.Length;
			item.PrimaryKey = fm.PrimaryKey;
			item.Format = fm.Format;
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
			throw new InvalidOperationException(string.Format(Resource.InvalidMemberInfoType,
				mi.Name,
				mi.MemberType));
		}

		private static ORMappingItemCollection InnerGetMappingInfoByObject<T>(T graph)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			System.Type type = null;

			if (typeof(T).IsInterface)
				type = typeof(T);
			else
				type = graph.GetType();

			return InnerGetMappingInfo(type);
		}

		private static ORMappingItemCollection InnerGetMappingInfo(System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			ORMappingItemCollection result = null;

			if (ORMappingContextCache.Instance.TryGetValue(type, out result) == false)
			{
				result = ORMappingsCache.Instance.GetOrAddNewValue(type,
					(cache, key) =>
					{
						ORMappingItemCollection mapping = GetMappingItemCollection(type);
						mapping.IsReadOnly = true;

						cache.Add(key, mapping);

						return mapping;
					});
			}

			return result;
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
					if (attr is SubClassORFieldMappingAttribute)
						result.SubClassFieldMappings.Add((SubClassORFieldMappingAttribute)attr);
					else
						if (attr is SubClassSqlBehaviorAttribute)
							result.SubClassFieldSqlBehaviors.Add((SubClassSqlBehaviorAttribute)attr);
						else
							if (attr is SubClassTypeAttribute)
								result.SubClassType = (SubClassTypeAttribute)attr;
							else
								if (attr is ORFieldMappingAttribute)
									result.FieldMapping = (ORFieldMappingAttribute)attr;
								else
									if (attr is SqlBehaviorAttribute)
										result.SqlBehavior = (SqlBehaviorAttribute)attr;
									else
										if (attr is SubClassPropertyEncryptionAttribute)
											result.SubClassPropertyEncryptions.Add((SubClassPropertyEncryptionAttribute)attr);
										else
											if (attr is PropertyEncryptionAttribute)
												result.PropertyEncryption = (PropertyEncryptionAttribute)attr;
			}

			return result;
		}
		#endregion 私有方法
	}
}
