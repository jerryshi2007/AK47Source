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
    /// ���л�ִ����
    /// </summary>
    /// <remarks>���л�ִ����</remarks>
    public static class JSONSerializerExecute
    {
        private static readonly string RegisterTypeToClientKey = (new object()).GetHashCode().ToString();
        private static string ContextConverterTypeCacheKey = "JSONSerializerContextTypeKey";
        private static readonly Dictionary<string, object> _EmptyDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// ��ͻ���ע������
        /// </summary>
        /// <param name="page">ҳ�����</param>
        /// <param name="typeKey">���ͱ�־Key</param>
        /// <param name="type">ע�������</param>
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
        /// ע��JavascriptConvter
        /// </summary>
        /// <param name="converterType">JavascriptConvter����</param>
        public static void RegisterConverter(Type converterType)
        {
            ExceptionHelper.FalseThrow(typeof(JavaScriptConverter).IsAssignableFrom(converterType),
                string.Format(Resources.DeluxeJsonResource.E_NotJavaScriptConverter, converterType.AssemblyQualifiedName));

            lock (JSONSerializerFactory.S_GlobalConverterTypesCache)
            {
                if (JSONSerializerFactory.S_GlobalConverterTypesCache.ContainsKey(converterType) == false)
                {
                    JavaScriptConverter converter = Activator.CreateInstance(converterType) as JavaScriptConverter;
                    foreach (Type supportType in converter.SupportedTypes)
                    {
                        if (JSONSerializerFactory.S_GlobalCheckConverterTypesCache.ContainsKey(supportType) == false)
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
        /// ׼����ʼע��������Converter
        /// </summary>
        public static void BeginRegisterContextConverters()
        {
            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).TrueThrow(
                "�Ѿ�ִ����BeginRegisterContextConverters������û��ִ��EndRegisterContextConverters");

            Dictionary<Type, JavaScriptConverter> converters = new Dictionary<Type, JavaScriptConverter>();

            ObjectContextCache.Instance.Add(JSONSerializerExecute.ContextConverterTypeCacheKey, converters);
        }

        /// <summary>
        /// ע�������ĵ�Converter��Ȼ��ִ��Action��ִ�к�ָ������ġ�
        /// </summary>
        /// <param name="action"></param>
        /// <param name="converterTypes"></param>
        public static void DoContextConvertersAction(Action action, params Type[] converterTypes)
        {
            if (action != null && converterTypes != null)
            {
                BeginRegisterContextConverters();

                try
                {
                    foreach (Type converterType in converterTypes)
                        RegisterContextConverter(converterType);

                    action();
                }
                finally
                {
                    EndRegisterContextConverters();
                }
            }
        }

        /// <summary>
        /// ��Converterע�ᵽ��������
        /// </summary>
        /// <param name="converterType"></param>
        public static void RegisterContextConverter(Type converterType)
        {
            converterType.NullCheck("converterType");

            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).FalseThrow(
                "û��ִ��BeginRegisterContextConverters������ִ��RegisterContextConverter");

            ExceptionHelper.FalseThrow(typeof(JavaScriptConverter).IsAssignableFrom(converterType),
                string.Format(Resources.DeluxeJsonResource.E_NotJavaScriptConverter, converterType.AssemblyQualifiedName));

            Dictionary<Type, JavaScriptConverter> converters = (Dictionary<Type, JavaScriptConverter>)ObjectContextCache.Instance[JSONSerializerExecute.ContextConverterTypeCacheKey];

            converters[converterType] = (JavaScriptConverter)TypeCreator.CreateInstance(converterType);
        }

        /// <summary>
        /// ����ע��������Converter
        /// </summary>
        public static void EndRegisterContextConverters()
        {
            ObjectContextCache.Instance.ContainsKey(JSONSerializerExecute.ContextConverterTypeCacheKey).FalseThrow(
                "û��ִ��BeginRegisterContextConverters������ִ��EndRegisterContextConverters");

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
        /// ���л�����
        /// </summary>
        /// <param name="input">Ҫ���л��Ķ���</param>
        /// <returns>���л����</returns>
        /// <remarks>���ϵͳ�ṩ�����л�����</remarks>
        public static string Serialize(object input)
        {
            return Serialize(input, null);
        }

        /// <summary>
        /// ���л�����
        /// </summary>
        /// <param name="input">Ҫ���л��Ķ���</param>
        /// <param name="type">Ҫ���л������ͣ�������input�Ļ����inputʵ�ֵĽӿ�</param>
        /// <returns>���л����</returns>
        public static string Serialize(object input, Type type)
        {
            return Serialize(input, type, 0);
        }

        /// <summary>
        /// ���л�����
        /// </summary>
        /// <param name="input">Ҫ���л��Ķ���</param>
        /// <param name="type">Ҫ���л������ͣ�������input�Ļ����inputʵ�ֵĽӿ�</param>
        /// <param name="resolverTypeLevel">Ҫ���������Ϣ�ļ������</param>
        /// <returns>���л����</returns>
        /// <remarks>���ϵͳ�ṩ�����л�����</remarks>
        public static string Serialize(object input, Type type, int resolverTypeLevel)
        {
            JavaScriptSerializer serializer = resolverTypeLevel > 0 ?
                JSONSerializerFactory.GetJavaScriptSerializer(input.GetType()) :
                JSONSerializerFactory.GetJavaScriptSerializer();

            serializer.MaxJsonLength = JSONSerializerFactory.GetMaxJsonLength();
            input = PreSerializeObject(input, type, resolverTypeLevel - 1);

            return serializer.Serialize(input);
        }

        /// <summary>
        /// ���л�������а���__type��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="addPrimitiveConverters">�Ƿ�����ԭʼ���͵�ת����</param>
        /// <returns></returns>
        public static string SerializeWithType(object input, bool addPrimitiveConverters = false)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());

            serializer.MaxJsonLength = JSONSerializerFactory.GetMaxJsonLength();
            JSONSerializerFactory.RegisterConverters(serializer);

            if (addPrimitiveConverters)
                serializer.RegisterConverters(InternalDateTimeConverter.Instances);

            return serializer.Serialize(input);
        }

        /// <summary>
        /// Ԥ�������л�����ͨ�����ô˺������ɶ����л�����Ԥ����
        /// </summary>
        /// <param name="input">Ҫ���л��Ķ���</param>
        /// <returns>������</returns>
        /// <remarks>Ԥ�������л�����ͨ�����ô˺������ɶ����л�����Ԥ����</remarks>
        public static object PreSerializeObject(object input)
        {
            return PreSerializeObject(input, null, 0);
        }

        /// <summary>
        /// Ԥ�������л�����ͨ�����ô˺������ɶ����л�����Ԥ����
        /// </summary>
        /// <param name="input">Ҫ���л��Ķ���</param>
        /// <param name="targetType">Ҫ���л������ͣ�������input�Ļ����inputʵ�ֵĽӿ�</param>
        /// <param name="resolverTypeLevel">Ҫ���������Ϣ�ļ������</param>
        /// <returns>������</returns>
        /// <remarks>Ԥ�������л�����ͨ�����ô˺������ɶ����л�����Ԥ����</remarks>
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
                    if ((input is ValueType) == false && (input is string) == false && (input is IDictionary) == false)
                    {
                        if (input is IEnumerable)
                            result = SerializeEnumerableData((IEnumerable)input, targetType, resolverTypeLevel);
                        else
                            result = SerializeReferenceData(input, targetType, resolverTypeLevel);
                    }
                }
            }

            if (resolverTypeLevel > 0 && result is IDictionary<string, object>)
            {
                Dictionary<string, object> resultDict = result as Dictionary<string, object>;

                if (resultDict.ContainsKey("__type") == false)
                    resultDict["__type"] = input.GetType().AssemblyQualifiedName;
            }

            return result;
        }

        /// <summary>
        /// ���л���ö�ٵĶ���
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetType"></param>
        /// <param name="resolverTypeLevel"></param>
        /// <returns></returns>
        private static object SerializeEnumerableData(IEnumerable input, Type targetType, int resolverTypeLevel)
        {
            ArrayList list = new ArrayList();

            foreach (object o in input)
                list.Add(PreSerializeObject(o, null, resolverTypeLevel - 1));

            return list;
        }

        /// <summary>
        /// ���л��������͵����ݣ���object�����ģ�
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
        /// ������inputת�������Ϊtype�Ķ����������ֱ��ת�������JSONת��
        /// </summary>
        /// <param name="input">Ҫת���Ķ���</param>
        /// <param name="type">Ҫת���ɵ�����</param>
        /// <returns>ת�����</returns>
        /// <remarks>������inputת�������Ϊtype�Ķ����������ֱ��ת�������JSONת��</remarks>
        [Obsolete("��ʹ��DeserializeObject")]
        public static object ConverterObject(object input, Type type)
        {
            return DeserializeObject(input, type, 0);
        }

        /// <summary>
        /// ���ַ��������л�Ϊ����������������ͻᾭ���ڲ���ת�������˷�����������js��ʹ��
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeString(string input, Type type)
        {
            type.NullCheck("type");

            JavaScriptSerializer serializer = JSONSerializerFactory.GetJavaScriptSerializer(type);

            serializer.RegisterConverters(InternalDateTimeConverter.Instances);

            return serializer.Deserialize(input, type);
        }

        public static T DeserializeString<T>(string input)
        {
            Type type = typeof(T);

            JavaScriptSerializer serializer = JSONSerializerFactory.GetJavaScriptSerializer(type);

            serializer.RegisterConverters(InternalDateTimeConverter.Instances);

            return (T)serializer.Deserialize(input, type);
        }

        /// <summary>
        /// ������inputת�������ΪT�Ķ����������ֱ��ת�������JSONת��
        /// </summary>
        /// <typeparam name="T">ת������</typeparam>
        /// <param name="input">Ҫת���Ķ���</param>
        /// <returns>ת�����</returns>
        public static T Deserialize<T>(object input)
        {
            return (T)DeserializeObject(input, typeof(T));
        }

        /// <summary>
        /// ������inputת�������Ϊtype�Ķ����������ֱ��ת�������JSONת��
        /// </summary>
        /// <param name="input">Ҫת���Ķ���</param>
        /// <param name="type">Ҫת���ɵ�����</param>
        /// <returns>ת�����</returns>
        /// <remarks>������inputת�������Ϊtype�Ķ����������ֱ��ת�������JSONת��</remarks>
        public static object DeserializeObject(object input, Type type)
        {
            //type = GetDeserializeObjectType(input, type);
            return DeserializeObject(input, type, 0);
        }

        /// <summary>
        /// ��json�ַ��������л��ɶ���json�������__type��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object DeserializeObject(string input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());

            serializer.MaxJsonLength = JSONSerializerFactory.GetMaxJsonLength();
            JSONSerializerFactory.RegisterConverters(serializer);

            return serializer.DeserializeObject(input);
        }

        /// <summary>
        /// ������inputת�������Ϊtype�Ķ����������ֱ��ת�������JSONת��
        /// </summary>
        /// <param name="input">Ҫת���Ķ���</param>
        /// <param name="type">Ҫת���ɵ�����</param>
        /// <param name="level">���ü���</param>
        /// <returns>ת�����</returns>
        private static object DeserializeObject(object input, Type type, int level)
        {
            if (level > 3)
            {
                //�ص㿼��Ŀ������Ҳ��Dictionary<string, object>�����
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
                    else if (type.IsPrimitive && typeof(IConvertible).IsAssignableFrom(type))
                    {
                        //if (type.GetInterface(typeof(IConvertible).AssemblyQualifiedName) != null)
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
                            Dictionary<string, object> dictionary = input as Dictionary<string, object>;

                            if (type != null && dictionary != null && dictionary.ContainsKey("__type") == false)
                                dictionary["__type"] = type.AssemblyQualifiedName;

                            JavaScriptSerializer serializer = JSONSerializerFactory.GetJavaScriptSerializer(type);

                            if (dictionary != null)
                                serializer.RegisterConverters(InternalDateTimeConverter.Instances);

                            string str = string.Empty;

                            if (input is string)
                                str = (string)input;
                            else
                                str = serializer.Serialize(input);

                            result = serializer.DeserializeObject(str);
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
                    ins.SetValue(DeserializeObject(objs[i], eltType), i);

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
