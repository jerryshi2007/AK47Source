using System;
using System.Text;
using System.Globalization;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Configuration;
using System.Diagnostics;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// JSON序列化器的工厂类
    /// </summary>
    /// <remarks>JSON序列化器的工厂类</remarks>
    public static class JSONSerializerFactory
    {
        internal static Dictionary<Type, Type> S_GlobalConverterTypesCache = new Dictionary<Type, Type>();
        internal static Dictionary<Type, Type> S_GlobalCheckConverterTypesCache = new Dictionary<Type, Type>();
        internal static ConfigurationSection S_GlobalConverterSection = null;
        internal static List<JavaScriptConverter> S_GlobalConvertersInConfig = new List<JavaScriptConverter>(10);

        /// <summary>
        /// 获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// </summary>
        /// <returns>JavaScriptSerializer对象</returns>
        /// <remarks>获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象</remarks>
        public static JavaScriptSerializer GetJavaScriptSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializer.MaxJsonLength = GetMaxJsonLength();

            RegisterConverters(serializer);

            return serializer;
        }

        /// <summary>
        /// 获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// 并序列化器专门序列化resolverType类别的对象
        /// </summary>
        /// <param name="resolverType">要序列化数据的类型</param>
        /// <returns>JavaScriptSerializer对象</returns>
        /// <remarks>获取一个新的序列化器JavaScriptSerializer对象，此序列化器已经注册了在配置文件中配置的Converter对象
        /// 并序列化器专门序列化resolverType类别的对象</remarks>
        public static JavaScriptSerializer GetJavaScriptSerializer(Type resolverType)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new DefaultTypeResolver(resolverType));

            serializer.MaxJsonLength = GetMaxJsonLength();

            RegisterConverters(serializer);

            return serializer;
        }

        /// <summary>
        /// 从配置文件中获取支持converterType类别的Converter转换器
        /// </summary>
        /// <param name="converterType">转换器匹配的类别</param>
        /// <returns>Converter转换器</returns>
        /// <remarks>从配置文件中获取支持converterType类别的Converter转换器</remarks>
        public static JavaScriptConverter GetJavaScriptConverter(Type converterType)
        {
            return GetJavaScriptConverter(converterType, converterType, null);
        }

        /// <summary>
        /// 从配置文件中读取配置的最大Json尺寸
        /// </summary>
        /// <returns></returns>
        public static int GetMaxJsonLength()
        {
            int result = -1;

            ScriptingJsonSerializationSection scriptSection = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("jsonSerialization");

            if (scriptSection == null)
                scriptSection = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("scriptJsonSerialization");

            if (scriptSection != null)
                result = scriptSection.MaxJsonLength;

            if (result < 0)
                result = int.MaxValue;

            return result;
        }

        public static JavaScriptConverter GetJavaScriptConverter(Type converterType, Type compareType, JavaScriptConverter[] converters)
        {
            JavaScriptConverter result = null;

            converters = converters ?? GetConverters();

            foreach (JavaScriptConverter c in converters)
            {
                foreach (Type t in c.SupportedTypes)
                {
                    if (t == compareType)
                    {
                        result = c;
                        break;
                    }
                }
            }

            return result;
        }

        internal static void RegisterConverters(JavaScriptSerializer serializer)
        {
            serializer.RegisterConverters(GetConverters());
        }

        private static JavaScriptConverter[] GetConverters()
        {
            JavaScriptConverter[] contextConverters = JSONSerializerExecute.GetContextConverters();
            JavaScriptConverter[] globalConverters = CreateGlobalCacheConverters();
            JavaScriptConverter[] configConverters = GetGlobalConvertersInConfig().ToArray();	//CreateConfigConverters();

            JavaScriptConverter[] converters = new JavaScriptConverter[contextConverters.Length + globalConverters.Length + configConverters.Length];

            globalConverters.CopyTo(converters, 0);
            configConverters.CopyTo(converters, globalConverters.Length);
            contextConverters.CopyTo(converters, globalConverters.Length + configConverters.Length);

            return converters;
        }

        private static JavaScriptConverter[] CreateGlobalCacheConverters()
        {
            List<JavaScriptConverter> converters = new List<JavaScriptConverter>();

            foreach (Type t in S_GlobalConverterTypesCache.Values)
                converters.Add((JavaScriptConverter)TypeCreator.CreateInstance(t));

            //converters.Add(InternalDateTimeConverter.Instance);

            return converters.ToArray();
        }

        /// <summary>
        /// 获取JsonSerializationSection
        /// </summary>
        /// <returns>JsonSerializationSection</returns>
        /// <remarks>获取JsonSerializationSection</remarks>
        private static ScriptingJsonSerializationSection GetJsonSerializationSection()
        {
            ScriptingJsonSerializationSection section = (ScriptingJsonSerializationSection)ConfigurationBroker.GetSection("scriptJsonSerialization");

            if (section == null)
                section = new ScriptingJsonSerializationSection();

            return section;
        }

        private static List<JavaScriptConverter> GetGlobalConvertersInConfig()
        {
            List<JavaScriptConverter> list = S_GlobalConvertersInConfig;

            lock (S_GlobalConvertersInConfig)
            {
                ScriptingJsonSerializationSection section = GetJsonSerializationSection();

                if (S_GlobalConverterSection != section)
                {
                    list.Clear();

                    foreach (Converter converter in section.Converters)
                    {
                        Type c = System.Web.Compilation.BuildManager.GetType(converter.Type, false);

                        if (c == null)
                        {
                            //沈峥调整，不抛出异常
                            //throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.DeluxeWebResource.E_UnknownType, new object[] { converter.Type }));

                            System.Exception ex = new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.DeluxeJsonResource.E_UnknownType, new object[] { converter.Type }));
                            ExceptionHelper.WriteToEventLog(ex, "JavaScriptConverter", EventLogEntryType.Warning);
                        }
                        else
                            if (!typeof(JavaScriptConverter).IsAssignableFrom(c))
                            {
                                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.DeluxeJsonResource.E_NotJavaScriptConverter, new object[] { c.Name }));
                            }

                        if (c != null)
                            list.Add((JavaScriptConverter)TypeCreator.CreateInstance(c));
                    }

                    S_GlobalConverterSection = section;
                }

                return list;
            }
        }
    }
}
