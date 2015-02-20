using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// 序列化执行器
    /// </summary>
    /// <remarks>序列化执行器</remarks>
    public static class JSONSerializerExecute
    {
        private static readonly string RegisterTypeToClientKey = (new object()).GetHashCode().ToString();
        private static string ContextConverterTypeCacheKey = "JSONSerializerContextTypeKey";
        private static readonly Dictionary<string, object> _EmptyDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 向客户端注册类型
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="typeKey">类型标志Key</param>
        /// <param name="type">注册的类型</param>
        public static void RegisterTypeToClient(System.Web.UI.Page page, string typeKey, Type type)
        {
            string key = RegisterTypeToClientKey + typeKey;
            if (page.Items.Contains(key))
            {
                ExceptionHelper.FalseThrow((Type)page.Items[key] == type,
                    Resources.DeluxeJsonResource.E_TypeKeyExist, typeKey);
            }
            else
            {
                page.Items[key] = type;
                page.ClientScript.RegisterStartupScript(typeof(object), key,
                    string.Format("if (typeof ($HGRootNS) != \"undefined\") $HGRootNS.{0} = \"{1}\";", typeKey, type.AssemblyQualifiedName), true);
            }
        }

        /// <summary>
        /// 注册JavascriptConvter
        /// </summary>
        /// <param name="converterType">JavascriptConvter类型</param>
        public static void RegisterConverter(Type converterType)
        {
            ExceptionHelper.FalseThrow(typeof(JavaScriptConverter).IsAssignableFrom(converterType),
                string.Format(Resources.DeluxeJsonResource.E_NotJavaScriptConverter, converterType.AssemblyQualifiedName));

            lock (JSONSerializerFactory.S_GlobalConverterTypesCache)
            {
                if (!JSONSerializerFactory.S_GlobalConverterTypesCache.ContainsKey(converterType))
                {
                    JavaScriptConverter converter = Activator.CreateInstance(converterType) as JavaScriptConverter;
                    foreach (Type supportType in converter.SupportedTypes)
                    {
                        if (!JSONSerializerFactory.S_GlobalCheckConverterTypesCache.ContainsKey(supportType))
                        {
                            JSONSerializerFactory.S_GlobalCheckConverterTypesCache.Add(supportType, converterType);
                        }
                        else
                        {
                            Type originalConverterType = JSONSerializerFactory.S_GlobalCheckConverterTypesCache[supportType];

                            ExceptionHelper.TrueThrow(originalConverterType != converterType,
                                Resources.DeluxeJsonResource.E_ConverterConflict,
                                originalConverterType,
                                converterType,
                                supportType);
                        }
                    }
                    JSONSerializerFactory.S_GlobalConverterTypesCache.Add(converterType, converterType);
                }
            }
        }

        /// <summary>
        /// 准备开始注册上下文Converter
        /// </summary>
        public static void BeginRegisterContextConverters()
        {
            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).TrueThrow(
                "已经执行了BeginRegisterContextConverters，但是没有执行EndRegisterContextConverters");

            Dictionary<Type, JavaScriptConverter> converters = new Dictionary<Type, JavaScriptConverter>();

            ObjectContextCache.Instance.Add(JSONSerializerExecute.ContextConverterTypeCacheKey, converters);
        }

        /// <summary>
        /// 将Converter注册到上下文中
        /// </summary>
        /// <param name="converterType"></param>
        public static void RegisterContextConverter(Type converterType)
        {
            converterType.NullCheck("converterType");

            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).FalseThrow(
                "没有执行BeginRegisterContextConverters，不能执行RegisterContextConverter");

            ExceptionHelper.FalseThrow(typeof(JavaScriptConverter).IsAssignableFrom(converterType),
                string.Format(Resources.DeluxeJsonResource.E_NotJavaScriptConverter, converterType.AssemblyQualifiedName));

            Dictionary<Type, JavaScriptConverter> converters = (Dictionary<Type, JavaScriptConverter>)ObjectContextCache.Instance[JSONSerializerExecute.ContextConverterTypeCacheKey];

            converters[converterType] = (JavaScriptConverter)TypeCreator.CreateInstance(converterType);
        }

        /// <summary>
        /// 结束注册上下文Converter
        /// </summary>
        public static void EndRegisterContextConverters()
        {
            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).FalseThrow(
                "没有执行BeginRegisterContextConverters，不能执行EndRegisterContextConverters");

            ObjectContextCache.Instance.Remove(JSONSerializerExecute.ContextConverterTypeCacheKey);
        }

        internal static JavaScriptConverter[] GetContextConverters()
        {
            Dictionary<Type, JavaScriptConverter> converters = null;

            if (ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey))
                converters = (Dictionary<Type, JavaScriptConverter>)ObjectContextCache.Instance[JSONSerializerExecute.ContextConverterTypeCacheKey];

            List<JavaScriptConverter> list = new List<JavaScriptConverter>();

            if (converters != null)
            {
                foreach (KeyValuePair<Type, JavaScriptConverter> kv in converters)
                    list.Add(kv.Value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <returns>序列化结果</returns>
        /// <remarks>替代系统提供的序列化调用</remarks>
        public static string Serialize(object input)
        {
            return Serialize(input, null);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="type">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <returns>序列化结果</returns>
        public static string Serialize(object input, Type type)
        {
            return Serialize(input, type, 0);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="type">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <param name="resolverTypeLevel">要输出类型信息的级别深度</param>
        /// <returns>序列化结果</returns>
        /// <remarks>替代系统提供的序列化调用</remarks>
        public static string Serialize(object input, Type type, int resolverTypeLevel)
        {
            JavaScriptSerializer ser = resolverTypeLevel > 0 ?
                JSONSerializerFactory.GetJavaScriptSerializer(input.GetType()) :
               JSONSerializerFactory.GetJavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            input = PreSerializeObject(input, type, resolverTypeLevel - 1);

            string result = ser.Serialize(input);

            return result;
        }

        /// <summary>
        /// 序列化，结果中包含__type信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SerializeWithType(object input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());
            JSONSerializerFactory.RegisterConverters(serializer);
            return serializer.Serialize(input);
        }

        /// <summary>
        /// 预处理序列化对象，通过调用此函数，可对序列化进行预处理
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <returns>处理结果</returns>
        /// <remarks>预处理序列化对象，通过调用此函数，可对序列化进行预处理</remarks>
        public static object PreSerializeObject(object input)
        {
            return PreSerializeObject(input, null, 0);
        }

        /// <summary>
        /// 预处理序列化对象，通过调用此函数，可对序列化进行预处理
        /// </summary>
        /// <param name="input">要序列化的对象</param>
        /// <param name="targetType">要序列化的类型，可以是input的基类或input实现的接口</param>
        /// <param name="resolverTypeLevel">要输出类型信息的级别深度</param>
        /// <returns>处理结果</returns>
        /// <remarks>预处理序列化对象，通过调用此函数，可对序列化进行预处理</remarks>
        public static object PreSerializeObject(object input, Type targetType, int resolverTypeLevel)
        {
            object result = input;
            if (input != null)
            {
                JavaScriptConverter converter = JSONSerializerFactory.GetJavaScriptConverter(input.GetType());
                if (converter != null)
                {
                    result = converter.Serialize(input, JSONSerializerFactory.GetJavaScriptSerializer());
                }
                else
                {
                    if (!(input is ValueType) && !(input is string) && !(input is IDictionary))
                    {
                        IEnumerable list = input as IEnumerable;
                        if (list != null)
                        {
                            result = SerializeEnumerableData(list, targetType, resolverTypeLevel);
                        }
                        else
                        {
                            result = SerializeReferenceData(input, targetType, resolverTypeLevel);
                        }
                    }
                }
            }

            if (resolverTypeLevel > 0 && result is Dictionary<string, object>)
            {
                Dictionary<string, object> resultDict = result as Dictionary<string, object>;
                if (!resultDict.ContainsKey("__type"))
                    resultDict["__type"] = input.GetType().AssemblyQualifiedName;
            }

            return result;
        }

        /// <summary>
        /// 序列化可枚举的对象
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetType"></param>
        /// <param name="resolverTypeLevel"></param>
        /// <returns></returns>
        private static object SerializeEnumerableData(IEnumerable input, Type targetType, int resolverTypeLevel)
        {
            //Dictionary<string, object> result = SerializeReferenceData(input, targetType, resolverTypeLevel);

            ArrayList list = new ArrayList();

            foreach (object o in input)
            {
                list.Add(PreSerializeObject(o, null, resolverTypeLevel - 1));
            }

            return list;
        }

        /// <summary>
        /// 序列化引用类型的数据（从object派生的）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetType"></param>
        /// <param name="resolverTypeLevel"></param>
        /// <returns></returns>
        private static Dictionary<string, object> SerializeReferenceData(object input, Type targetType, int resolverTypeLevel)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            targetType = NormalizeTargetType(input, targetType);

            foreach (PropertyInfo pi in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                ScriptIgnoreAttribute ignoreAttr = (ScriptIgnoreAttribute)Attribute.GetCustomAttribute(pi, typeof(ScriptIgnoreAttribute), false);

                if (ignoreAttr == null)
                {
                    MethodInfo mi = pi.GetGetMethod();
                    if (mi != null && mi.GetParameters().Length <= 0)
                    {
                        object v = PreSerializeObject(mi.Invoke(input, null), null, resolverTypeLevel - 1); ;

                        result.Add(ClientPropertyNameAttribute.GetClientPropertyName(pi), v);
                    }
                }
            }

            return result;
        }

        private static Type NormalizeTargetType(object input, Type targetType)
        {
            Type inputType = input.GetType();

            if (targetType != null)
            {
                if (targetType.IsAssignableFrom(inputType) == false)
                    throw new SystemSupportException(string.Format(Resources.DeluxeJsonResource.E_NoInheritFrom, inputType, targetType));
            }
            else
                targetType = inputType;

            return targetType;
        }

        /// <summary>
        /// 将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="type">要转换成的类型</param>
        /// <returns>转换结果</returns>
        /// <remarks>将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换</remarks>
        [Obsolete("请使用DeserializeObject")]
        public static object ConverterObject(object input, Type type)
        {
            return DeserializeObject(input, type, 0);
        }

        /// <summary>
        /// 将对象input转换成类别为T的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="input">要转换的对象</param>
        /// <returns>转换结果</returns>
        public static T Deserialize<T>(object input)
        {
            return (T)DeserializeObject(input, typeof(T));
        }

        /// <summary>
        /// 将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="type">要转换成的类型</param>
        /// <returns>转换结果</returns>
        /// <remarks>将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换</remarks>
        public static object DeserializeObject(object input, Type type)
        {
            //type = GetDeserializeObjectType(input, type);
            return DeserializeObject(input, type, 0);
        }

        /// <summary>
        /// 将json字符串反序列化成对象，json中须包含__type信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object DeserializeObject(string input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());
            JSONSerializerFactory.RegisterConverters(serializer);

            return serializer.DeserializeObject(input);
        }

        /// <summary>
        /// 将对象input转换成类别为type的对象，如果不能直接转换则调用JSON转换
        /// </summary>
        /// <param name="input">要转换的对象</param>
        /// <param name="type">要转换成的类型</param>
        /// <param name="level">调用级别</param>
        /// <returns>转换结果</returns>
        private static object DeserializeObject(object input, Type type, int level)
        {
            if (level > 3)
            {
                //重点考虑目标类型也是Dictionary<string, object>的情况
                if (type != typeof(Dictionary<string, object>))
                    throw new ApplicationException(string.Format(Resources.DeluxeJsonResource.E_InvokeLevelTooDeep, level));
                else
                    return input;
            }

            object result = null;

            if (input != null) //Added by shenzheng 2008-3-10
            {
                if (type.IsEnum)
                {
                    result = DataConverter.ChangeType(input, type);
                }
                else
                {
                    if (type.IsAssignableFrom(input.GetType()) && type != typeof(Dictionary<string, object>))
                    {
                        result = input;
                    }
                    else if (type.GetInterface(typeof(IConvertible).AssemblyQualifiedName) != null)
                    {
                        result = Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        if (input is object[])
                        {
                            object tempResult = DeserializeArrayObject((object[])input, type, level);

                            if (tempResult == null)
                                throw new ApplicationException(string.Format(Resources.DeluxeJsonResource.E_CanNotConvertTypeTo, input.GetType().AssemblyQualifiedName, type.AssemblyQualifiedName));
                            else
                                result = tempResult;
                        }
                        else if (type.IsEnum)
                        {
                            result = DataConverter.ChangeType(input.GetType(), input, type);
                        }
                        else
                        {
                            Dictionary<string, object> dic = input as Dictionary<string, object>;

                            if (type != null && dic != null && !dic.ContainsKey("__type"))
                            {
                                dic["__type"] = type.AssemblyQualifiedName;
                            }

                            JavaScriptSerializer ser = JSONSerializerFactory.GetJavaScriptSerializer(type);

                            //if (dic != null)
                            ser.RegisterConverters(new JavaScriptConverter[] { InternalDateTimeConverter.Instance });

                            string str = input is string ? (string)input : ser.Serialize(input);

                            result = ser.DeserializeObject(str);
                        }

                        result = DeserializeObject(result, type, ++level);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedData"></param>
        /// <param name="target"></param>
        public static void FillDeserializedCollection<T>(object serializedData, EditableDataObjectCollectionBase<T> target)
        {
            if (serializedData != null)
            {
                T[] deserializedArray = JSONSerializerExecute.Deserialize<T[]>(serializedData);

                target.Clear();

                deserializedArray.ForEach(d => target.Add(d));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedData"></param>
        /// <param name="target"></param>
        public static void FillDeserializedCollection<T>(object serializedData, IList<T> target)
        {
            if (serializedData != null)
            {
                T[] deserializedArray = JSONSerializerExecute.Deserialize<T[]>(serializedData);

                target.Clear();

                deserializedArray.ForEach(d => target.Add(d));
            }
        }

        public static void FillDeserializedDictionary(IDictionary<string, object> serializedData, string key, IDictionary<string, object> target)
        {
            if (serializedData != null)
            {
                IDictionary<string, object> sourceValue = serializedData.GetValue(key, _EmptyDictionary);

                sourceValue.ForEach(kp => target.Add(kp.Key, kp.Value));
            }
        }

        //private static Type GetDeserializeObjectType(object input, Type type)
        //{
        //    string str = input as string;
        //    if (str != null) input = JSONSerializerFactory.GetJavaScriptSerializer(typeof(Dictionary<string, object>)).DeserializeObject(str);

        //    Dictionary<string, object> dic = input as Dictionary<string, object>;

        //    if (dic != null)
        //    {
        //        string typeStr = dic["__type"] as string;

        //        if (!string.IsNullOrEmpty(typeStr))
        //        {
        //            type = Type.GetType(typeStr);
        //        }
        //    }

        //    return type;
        //}

        private static object DeserializeArrayObject(object[] input, Type type, int level)
        {
            object tempResult = null;
            if (typeof(Array).IsAssignableFrom(type))
            {
                Type eltType = type.GetMethod("Get", new Type[1] { typeof(int) }).ReturnType;
                object[] objs = input;
                Array ins = Array.CreateInstance(eltType, objs.Length);

                for (int i = 0; i < objs.Length; i++)
                {
                    ins.SetValue(DeserializeObject(objs[i], eltType), i);
                }
                tempResult = ins;
            }
            else if (typeof(ICollection<object>).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                ICollection<object> c = (ICollection<object>)ins;
                foreach (object o in input)
                    c.Add(DeserializeObject(o, type.GetGenericArguments()[0], level));
                tempResult = ins;
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                IList l = (IList)ins;
                MethodInfo mi = type.GetMethod("get_Item", new Type[1] { typeof(int) });
                if (mi != null)
                {
                    Type eltType = mi.ReturnType;
                    foreach (object o in input)
                        l.Add(DeserializeObject(o, eltType, level));
                    tempResult = ins;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                object ins = TypeCreator.CreateInstance(type);
                MethodInfo mi = type.GetMethod("get_Item", new Type[1] { typeof(int) });
                if (mi != null)
                {
                    Type eltType = mi.ReturnType;
                    foreach (object o in input)
                        mi.Invoke(ins, new object[1] { DeserializeObject(o, eltType, level) });
                    tempResult = ins;
                }
            }

            return tempResult;
        }
    }
}
