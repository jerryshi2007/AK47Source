using MCS.Web.Library.Script.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Web.Library.Script.Mechanism
{
    public class JavaScriptSerializer
    {
        // Fields
        private Dictionary<Type, JavaScriptConverter> _converters;
        private int _maxJsonLength;
        private int _recursionLimit;
        private JavaScriptTypeResolver _typeResolver;
        internal static readonly long DatetimeMinTimeTicks;
        internal const int DefaultMaxJsonLength = 0x200000;
        internal const int DefaultRecursionLimit = 100;
        internal const string ServerTypeFieldName = "__type";

        // Methods
        static JavaScriptSerializer()
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DatetimeMinTimeTicks = time.Ticks;
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public JavaScriptSerializer()
            : this(null)
        {
        }

        public JavaScriptSerializer(JavaScriptTypeResolver resolver)
        {
            this._typeResolver = resolver;
            this.RecursionLimit = 100;
            this.MaxJsonLength = 0x200000;
        }

        private bool CheckScriptIgnoreAttribute(MemberInfo memberInfo)
        {
            if (memberInfo.IsDefined(typeof(ScriptIgnoreAttribute), true))
            {
                return true;
            }

            ScriptIgnoreAttribute attribute =
                (ScriptIgnoreAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(ScriptIgnoreAttribute), true);

            return ((attribute != null) && attribute.ApplyToOverrides);
        }

        internal bool ConverterExistsForType(Type t, out JavaScriptConverter converter)
        {
            converter = this.GetConverter(t);
            return (converter != null);
        }

        public T ConvertToType<T>(object obj)
        {
            return (T)ObjectConverter.ConvertObjectToType(obj, typeof(T), this);
        }

        public object ConvertToType(object obj, Type targetType)
        {
            return ObjectConverter.ConvertObjectToType(obj, targetType, this);
        }

        public T Deserialize<T>(string input)
        {
            return (T)Deserialize(this, input, typeof(T), this.RecursionLimit);
        }

        public object Deserialize(string input, Type targetType)
        {
            return Deserialize(this, input, targetType, this.RecursionLimit);
        }

        internal static object Deserialize(JavaScriptSerializer serializer, string input, Type type, int depthLimit)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (input.Length > serializer.MaxJsonLength)
            {
                throw new ArgumentException(AtlasWeb.JSON_MaxJsonLengthExceeded, "input");
            }
            return ObjectConverter.ConvertObjectToType(JavaScriptObjectDeserializer.BasicDeserialize(input, depthLimit, serializer), type, serializer);
        }

        public object DeserializeObject(string input)
        {
            return Deserialize(this, input, null, this.RecursionLimit);
        }

        private JavaScriptConverter GetConverter(Type t)
        {
            if (this._converters != null)
            {
                while (t != null)
                {
                    if (this._converters.ContainsKey(t))
                    {
                        return this._converters[t];
                    }
                    t = t.BaseType;
                }
            }
            return null;
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException("converters");
            }
            foreach (JavaScriptConverter converter in converters)
            {
                IEnumerable<Type> supportedTypes = converter.SupportedTypes;
                if (supportedTypes != null)
                {
                    foreach (Type type in supportedTypes)
                    {
                        this.Converters[type] = converter;
                    }
                    continue;
                }
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public string Serialize(object obj)
        {
            return this.Serialize(obj, SerializationFormat.JSON);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Serialize(object obj, StringBuilder output)
        {
            this.Serialize(obj, output, SerializationFormat.JSON);
        }

        internal string Serialize(object obj, SerializationFormat serializationFormat)
        {
            StringBuilder output = new StringBuilder();
            this.Serialize(obj, output, serializationFormat);
            return output.ToString();
        }

        internal void Serialize(object obj, StringBuilder output, SerializationFormat serializationFormat)
        {
            this.SerializeValue(obj, output, 0, null, serializationFormat, null);
            if ((serializationFormat == SerializationFormat.JSON) && (output.Length > this.MaxJsonLength))
            {
                throw new InvalidOperationException(AtlasWeb.JSON_MaxJsonLengthExceeded);
            }
        }

        private static void SerializeBoolean(bool o, StringBuilder sb)
        {
            if (o)
            {
                sb.Append("true");
            }
            else
            {
                sb.Append("false");
            }
        }

        private void SerializeCustomObject(object o, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            bool flag = true;
            Type type = o.GetType();
            sb.Append('{');
            if (this.TypeResolver != null)
            {
                string str = this.TypeResolver.ResolveTypeId(type);
                if (str != null)
                {
                    SerializeString("__type", sb);
                    sb.Append(':');
                    this.SerializeValue(str, sb, depth, objectsInUse, serializationFormat, null);
                    flag = false;
                }
            }
            foreach (FieldInfo info in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!this.CheckScriptIgnoreAttribute(info))
                {
                    if (!flag)
                    {
                        sb.Append(',');
                    }
                    SerializeString(info.Name, sb);
                    sb.Append(':');
                    this.SerializeValue(SecurityUtils.FieldInfoGetValue(info, o), sb, depth, objectsInUse, serializationFormat, info);
                    flag = false;
                }
            }
            foreach (PropertyInfo info2 in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                if (!this.CheckScriptIgnoreAttribute(info2))
                {
                    MethodInfo getMethod = info2.GetGetMethod();
                    if ((getMethod != null) && (getMethod.GetParameters().Length <= 0))
                    {
                        if (!flag)
                        {
                            sb.Append(',');
                        }
                        SerializeString(info2.Name, sb);
                        sb.Append(':');
                        this.SerializeValue(SecurityUtils.MethodInfoInvoke(getMethod, o, null), sb, depth, objectsInUse, serializationFormat, info2);
                        flag = false;
                    }
                }
            }
            sb.Append('}');
        }

        private static void SerializeDateTime(DateTime datetime, StringBuilder sb, SerializationFormat serializationFormat)
        {
            if (serializationFormat == SerializationFormat.JSON)
            {
                sb.Append("\"\\/Date(");
                sb.Append((long)((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710L));
                sb.Append(")\\/\"");
            }
            else
            {
                sb.Append("new Date(");
                sb.Append((long)((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 0x2710L));
                sb.Append(")");
            }
        }

        private void SerializeDictionary(IDictionary o, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            sb.Append('{');
            bool flag = true;
            bool flag2 = false;
            if (o.Contains("__type"))
            {
                flag = false;
                flag2 = true;
                this.SerializeDictionaryKeyValue("__type", o["__type"], sb, depth, objectsInUse, serializationFormat);
            }
            foreach (DictionaryEntry entry in o)
            {
                string key = entry.Key as string;
                if (key == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.JSON_DictionaryTypeNotSupported, new object[] { o.GetType().FullName }));
                }
                if (flag2 && string.Equals(key, "__type", StringComparison.Ordinal))
                {
                    flag2 = false;
                }
                else
                {
                    if (!flag)
                    {
                        sb.Append(',');
                    }
                    this.SerializeDictionaryKeyValue(key, entry.Value, sb, depth, objectsInUse, serializationFormat);
                    flag = false;
                }
            }
            sb.Append('}');
        }

        private void SerializeDictionaryKeyValue(string key, object value, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            SerializeString(key, sb);
            sb.Append(':');
            this.SerializeValue(value, sb, depth, objectsInUse, serializationFormat, null);
        }

        private void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat)
        {
            sb.Append('[');
            bool flag = true;
            foreach (object obj2 in enumerable)
            {
                if (!flag)
                {
                    sb.Append(',');
                }
                this.SerializeValue(obj2, sb, depth, objectsInUse, serializationFormat, null);
                flag = false;
            }
            sb.Append(']');
        }

        private static void SerializeGuid(Guid guid, StringBuilder sb)
        {
            sb.Append("\"").Append(guid.ToString()).Append("\"");
        }

        internal static string SerializeInternal(object o)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(o);
        }

        private static void SerializeString(string input, StringBuilder sb)
        {
            sb.Append('"');
            sb.Append(HttpUtility.JavaScriptStringEncode(input));
            sb.Append('"');
        }

        private static void SerializeUri(Uri uri, StringBuilder sb)
        {
            sb.Append("\"").Append(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped)).Append("\"");
        }

        private void SerializeValue(object o, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat, [Optional, DefaultParameterValue(null)] MemberInfo currentMember)
        {
            if (++depth > this._recursionLimit)
            {
                throw new ArgumentException(AtlasWeb.JSON_DepthLimitExceeded);
            }

            JavaScriptConverter converter = null;
            if ((o != null) && this.ConverterExistsForType(o.GetType(), out converter))
            {
                IDictionary<string, object> dictionary = converter.Serialize(o, this);
                if (this.TypeResolver != null)
                {
                    string str = this.TypeResolver.ResolveTypeId(o.GetType());
                    if (str != null)
                    {
                        dictionary["__type"] = str;
                    }
                }
                sb.Append(this.Serialize(dictionary, serializationFormat));
            }
            else
            {
                this.SerializeValueInternal(o, sb, depth, objectsInUse, serializationFormat, currentMember);
            }
        }

        private void SerializeValueInternal(object o, StringBuilder sb, int depth, Hashtable objectsInUse, SerializationFormat serializationFormat, MemberInfo currentMember)
        {
            if ((o == null) || DBNull.Value.Equals(o))
            {
                sb.Append("null");
            }
            else
            {
                string input = o as string;
                if (input != null)
                {
                    SerializeString(input, sb);
                }
                else if (o is char)
                {
                    if (((char)o) == '\0')
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        SerializeString(o.ToString(), sb);
                    }
                }
                else if (o is bool)
                {
                    SerializeBoolean((bool)o, sb);
                }
                else if (o is DateTime)
                {
                    SerializeDateTime((DateTime)o, sb, serializationFormat);
                }
                else if (o is DateTimeOffset)
                {
                    DateTimeOffset offset = (DateTimeOffset)o;
                    SerializeDateTime(offset.UtcDateTime, sb, serializationFormat);
                }
                else if (o is Guid)
                {
                    SerializeGuid((Guid)o, sb);
                }
                else
                {
                    Uri uri = o as Uri;
                    if (uri != null)
                    {
                        SerializeUri(uri, sb);
                    }
                    else if (o is double)
                    {
                        sb.Append(((double)o).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (o is float)
                    {
                        sb.Append(((float)o).ToString("r", CultureInfo.InvariantCulture));
                    }
                    else if (o.GetType().IsPrimitive || (o is decimal))
                    {
                        IConvertible convertible = o as IConvertible;
                        if (convertible != null)
                        {
                            sb.Append(convertible.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            sb.Append(o.ToString());
                        }
                    }
                    else
                    {
                        Type enumType = o.GetType();
                        if (enumType.IsEnum)
                        {
                            Type underlyingType = Enum.GetUnderlyingType(enumType);
                            if ((underlyingType == typeof(long)) || (underlyingType == typeof(ulong)))
                            {
                                string message = (currentMember != null) ? (string.Format(CultureInfo.CurrentCulture, AtlasWeb.JSON_CannotSerializeMemberGeneric, new object[] { currentMember.Name, currentMember.ReflectedType.FullName }) + " " + AtlasWeb.JSON_InvalidEnumType) : AtlasWeb.JSON_InvalidEnumType;
                                throw new InvalidOperationException(message);
                            }
                            sb.Append(((Enum)o).ToString("D"));
                        }
                        else
                        {
                            try
                            {
                                if (objectsInUse == null)
                                {
                                    objectsInUse = new Hashtable(new ReferenceComparer());
                                }
                                else if (objectsInUse.ContainsKey(o))
                                {
                                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.JSON_CircularReference, new object[] { enumType.FullName }));
                                }
                                objectsInUse.Add(o, null);
                                IDictionary dictionary = o as IDictionary;
                                if (dictionary != null)
                                {
                                    this.SerializeDictionary(dictionary, sb, depth, objectsInUse, serializationFormat);
                                }
                                else
                                {
                                    IEnumerable enumerable = o as IEnumerable;
                                    if (enumerable != null)
                                    {
                                        this.SerializeEnumerable(enumerable, sb, depth, objectsInUse, serializationFormat);
                                    }
                                    else
                                    {
                                        this.SerializeCustomObject(o, sb, depth, objectsInUse, serializationFormat);
                                    }
                                }
                            }
                            finally
                            {
                                if (objectsInUse != null)
                                {
                                    objectsInUse.Remove(o);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Properties
        private Dictionary<Type, JavaScriptConverter> Converters
        {
            get
            {
                if (this._converters == null)
                {
                    this._converters = new Dictionary<Type, JavaScriptConverter>();
                }

                return this._converters;
            }
        }

        public int MaxJsonLength
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._maxJsonLength;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(AtlasWeb.JSON_InvalidMaxJsonLength);
                }
                this._maxJsonLength = value;
            }
        }

        public int RecursionLimit
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._recursionLimit;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(AtlasWeb.JSON_InvalidRecursionLimit);
                }
                this._recursionLimit = value;
            }
        }

        internal JavaScriptTypeResolver TypeResolver
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._typeResolver;
            }
        }

        // Nested Types
        private class ReferenceComparer : IEqualityComparer
        {
            // Methods
            bool IEqualityComparer.Equals(object x, object y)
            {
                return (x == y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                int result = this.GetHashCode();

                if (obj != null)
                    result = obj.GetHashCode();

                return result;
            }
        }

        internal enum SerializationFormat
        {
            JSON,
            JavaScript
        }
    }


}
