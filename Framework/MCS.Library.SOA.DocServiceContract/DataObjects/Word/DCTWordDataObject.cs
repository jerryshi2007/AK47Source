using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MCS.Library.Core;
using System.Reflection;
using MCS.Library.SOA.DocServiceContract.DataObjects;
using System.Collections;
using System.Xml.Linq;

namespace MCS.Library.SOA.DocServiceContract
{
	[DataContract]
	public class DCTWordDataObject
	{
		public DCTWordDataObject()
		{
			propertyCollection = new DCTDataPropertyCollection();
		}

		private DCTDataPropertyCollection propertyCollection;
		[DataMember]
		public DCTDataPropertyCollection PropertyCollection
		{
			get { return propertyCollection; }
			set { propertyCollection = value; }
		}

		public void Load(object source)
		{
			ExceptionHelper.TrueThrow(source == null, "源对象不能为空");

			Type sourceType = source.GetType();

			PropertyInfo[] fieldInfos = sourceType.GetProperties();

			foreach (PropertyInfo propInfo in fieldInfos)
			{
				if (propInfo.PropertyType.IsValueType || propInfo.PropertyType == typeof(string))
				{
					DCTSimpleProperty simpleProp = ConvertToSimpleProperty(propInfo, source);
					//            object[] attrs = propInfo.GetCustomAttributes(typeof(WordPropertyAttribute), true);
					//WordPropertyAttribute attr = attrs.Length != 0 ? ((WordPropertyAttribute)attrs[0]) : new WordPropertyAttribute();
					if (simpleProp != null)
					{
						this.PropertyCollection.Add(simpleProp);
					}
				}
			}

		}

		public void LoadFromMappingFile(XElement node)
		{
			ExceptionHelper.TrueThrow(node == null, "映射信息不能为空。");
			DCTSimpleProperty simpleProp = ConvertToSimpleProperty(node);

			this.PropertyCollection.Add(simpleProp);
		}

		public void LoadCollection(ICollection collection, string tagID)
		{
			foreach (var obj in collection)
			{
				LoadComplexProperty(obj, tagID);
			}
		}

		public void LoadComplexProperty(object source, string tagID)
		{
			ExceptionHelper.TrueThrow(source == null, "源对象不能为空");

			DCTWordDataObject wordDataObj = new DCTWordDataObject();
			DCTComplexProperty complexProp = this.PropertyCollection[tagID] as DCTComplexProperty;
			if (null == complexProp)
			{
				complexProp = new DCTComplexProperty();
				complexProp.TagID = tagID;
				this.PropertyCollection.Add(complexProp);
			}

			Type sourceType = source.GetType();

			PropertyInfo[] fieldInfos = sourceType.GetProperties();

			foreach (PropertyInfo propInfo in fieldInfos)
			{
				if (propInfo.PropertyType.Name != typeof(object).Name)
				{
					DCTSimpleProperty simpleProp = ConvertToSimpleProperty(propInfo, source);
					if (simpleProp != null)
					{
						wordDataObj.PropertyCollection.Add(simpleProp);
					}
				}
			}
			complexProp.DataObjects.Add(wordDataObj);


		}

		private DCTSimpleProperty ConvertToSimpleProperty(PropertyInfo propInfo, object source)
		{
			DCTSimpleProperty simpleProp = null;
			var value = propInfo.GetValue(source, null);
			Type type = value.GetType();
			PropertyInfo[] propInfos = type.GetProperties();
			object[] attrs = propInfo.GetCustomAttributes(typeof(WordPropertyAttribute), true);

			if (attrs.Length > 0)
			{
				simpleProp = new DCTSimpleProperty();
				WordPropertyAttribute attr = (WordPropertyAttribute)attrs[0];
				simpleProp.TagID = attr.TagID;
				simpleProp.FormatString = attr.FormatString;
				simpleProp.IsReadOnly = attr.IsReadOnly;
				simpleProp.Value = propInfo.GetValue(source, null);
			}
			return simpleProp;
		}

		private DCTSimpleProperty ConvertToSimpleProperty(XElement propInfo)
		{
			DCTSimpleProperty simpleProp = new DCTSimpleProperty();
			string tagId = "", formatString = "", type = "";
			bool isReadOnly = false;
			object value = null;
			foreach (XAttribute attr in propInfo.Attributes())
			{
				switch (attr.Name.ToString())
				{
					case "TagID":
						tagId = attr.Value;
						break;
					case "FormatString":
						formatString = attr.Value;
						break;
					case "IsReadOnly":
						isReadOnly = bool.Parse(attr.Value);
						break;
					case "PropertyValue":
						value = attr.Value;
						break;
					case "PropertyValueType":
						type = attr.Value;
						break;
					default:
						break;
				}
			}
			simpleProp.FormatString = formatString;
			simpleProp.IsReadOnly = isReadOnly;
			simpleProp.TagID = tagId;
			simpleProp.Type = type;
			simpleProp.Value = value;
			return simpleProp;

		}

		public T To<T>() where T : new()
		{
			T t = new T();
			Type type = typeof(T);
			PropertyInfo[] props = type.GetProperties();

			foreach (PropertyInfo prop in props)
			{
				object[] attrs = prop.GetCustomAttributes(typeof(WordPropertyAttribute), true);
				if (attrs.Length != 0)
				{
					WordPropertyAttribute attr = (WordPropertyAttribute)attrs[0];
					string str = (string)((DCTSimpleProperty)this.PropertyCollection[attr.TagID]).Value;
					prop.SetValue(t, TryParse(str, prop.PropertyType), null);
				}

			}

			return t;
		}

		private static object TryParse(string val, Type type)
		{
			GeneralParserService parserService = GeneralParserService.Default;
			return parserService.Parse(val, type);
			//if (type == typeof(string))
			//    return val;
			//MethodInfo mi = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string), type.MakeByRefType() }, null);
			//if (null == mi)
			//    return null;
			//object outputResult = null;
			//object[] arguments = new object[] { val, outputResult };
			//bool parseStatus = (bool)mi.Invoke(null, arguments);
			//if (parseStatus)
			//{
			//    outputResult = arguments[1];
			//    return outputResult;
			//}
			//return null;
		}

		public List<T> ToList<T>(string tagID) where T : new()
		{
			List<T> results = new List<T>();
			Type type = typeof(T);
			PropertyInfo[] props = type.GetProperties();
			DCTDataProperty dctDataProp = this.PropertyCollection[tagID];
			ExceptionHelper.TrueThrow(dctDataProp == null || dctDataProp is DCTSimpleProperty, string.Format("无法根据{0}找到对应的DCTDataProperty", tagID));

			DCTComplexProperty compProp = (DCTComplexProperty)dctDataProp;

			foreach (DCTWordDataObject wordObj in compProp.DataObjects)
			{
				T t = new T();
				if (wordObj.PropertyCollection.Count == 0)
					continue;
				foreach (PropertyInfo prop in props)
				{
					object[] attrs = prop.GetCustomAttributes(typeof(WordPropertyAttribute), true);
					if (attrs.Length != 0)
					{
						WordPropertyAttribute attr = (WordPropertyAttribute)attrs[0];
						DCTSimpleProperty simpleProp = (DCTSimpleProperty)wordObj.PropertyCollection[attr.TagID];
						if (null == simpleProp)
							continue;
						string str = (string)((DCTSimpleProperty)wordObj.PropertyCollection[attr.TagID]).Value;
						prop.SetValue(t, TryParse(str, prop.PropertyType), null);
					}
				}

				results.Add(t);
			}
			return results;

		}
	}
}
