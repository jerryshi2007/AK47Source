﻿using MCS.Library.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MCS.Library.Core
{
    /// <summary>
    /// 为一般集合类所做的扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 转换成Javascript的日期对应的整数（从1970年1月1日开始的毫秒数）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToJavascriptDateNumber(this DateTime dt)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);

            return Convert.ToInt64((dt - baseTime).TotalMilliseconds);
        }

        /// <summary>
        /// Javascript的日期对应的整数（从1970年1月1日开始的毫秒数）转换成DateTime
        /// </summary>
        /// <param name="jsMilliseconds"></param>
        /// <returns></returns>
        public static DateTime JavascriptDateNumberToDateTime(this long jsMilliseconds)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return baseTime.AddMilliseconds(jsMilliseconds).ToLocalTime();
        }

        /// <summary>
        /// 将日期类型转换为一个Dictionary，里面包含Ticks和DateKind，主要是和Json序列化中的类型转换相关
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this DateTime dt)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["DateValue"] = dt.Ticks;
            dict["DateKind"] = dt.Kind;

            return dict;
        }

        /// <summary>
        /// 将一个Dictionary转换成DateTime，需要这个Dictionaty中有DateValue值(Ticks)和DateKind值(DateTimeKind)。
        /// 如果没有，则返回DateTime.MinValue
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this IDictionary<string, object> dictionary)
        {
            DateTime result = DateTime.MinValue;

            long ticks = dictionary.GetValue("DateValue", -1L);
            DateTimeKind kind = dictionary.GetValue("DateKind", DateTimeKind.Local);

            if (ticks != -1)
            {
                result = new DateTime(ticks, kind);

                if (result != DateTime.MinValue && result.Kind == DateTimeKind.Utc)
                    result = result.ToLocalTime();
            }

            return result;
        }

        /// <summary>
        /// 字符串不是Null且Empty
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string data)
        {
            bool result = false;

            if (data != null)
                result = (string.IsNullOrEmpty(data) == false);

            return result;
        }

        /// <summary>
        /// 如果字符串不为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNotEmpty(this string data, Action<string> action)
        {
            if (data.IsNotEmpty() && action != null)
                action(data);
        }

        /// <summary>
        /// 如果字符串不为空，则执行Func
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static R IsNotEmpty<R>(this string data, Func<string, R> func)
        {
            R result = default(R);

            if (data.IsNotEmpty() && func != null)
                result = func(data);

            return result;
        }

        /// <summary>
        /// 字符串是否为Null或Empty
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string data)
        {
            return string.IsNullOrEmpty(data);
        }

        /// <summary>
        /// 如果字符串为空，则返回替代的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="replacedData"></param>
        /// <returns></returns>
        public static string NullOrEmptyIs(this string data, string replacedData)
        {
            string result = data;

            if (result.IsNullOrEmpty())
                result = replacedData;

            return result;
        }

        /// <summary>
        /// 如果字符串为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNullOrEmpty(this string data, Action action)
        {
            if (string.IsNullOrEmpty(data) && action != null)
                action();
        }

        /// <summary>
        /// 字符串是否为Null、Empty和WhiteSpace
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string data)
        {
            return string.IsNullOrWhiteSpace(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNullOrWhiteSpace(this string data, Action action)
        {
            if (string.IsNullOrWhiteSpace(data) && action != null)
                action();
        }

        /// <summary>
        /// 字符串不是Null、Empty和WhiteSpace
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNotWhiteSpace(this string data)
        {
            bool result = false;

            if (data != null)
                result = (string.IsNullOrWhiteSpace(data) == false);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNotWhiteSpace(this string data, Action<string> action)
        {
            if (data.IsNotWhiteSpace() && action != null)
                action(data);
        }

        /// <summary>
        /// 大小写无关的比较
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int IgnoreCaseCompare(this string strA, string strB)
        {
            return string.Compare(strA, strB, true);
        }

        /// <summary>
        /// 如果对象为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNull(this object data, Action action)
        {
            if (data == null && action != null)
                action();
        }

        /// <summary>
        /// 如果对象不为空，则执行Action
        /// </summary>
        /// <typeparam name="T">对象的类型泛型</typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNotNull<T>(this T data, Action<T> action)
        {
            if (data != null && action != null)
                action(data);
        }

        /// <summary>
        /// 如果对象不为空，则执行Func，返回某个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static R IsNotNull<T, R>(this T data, Func<T, R> func)
        {
            R result = default(R);

            if (data != null && func != null)
                result = func(data);

            return result;
        }

        /// <summary>
        /// 如果时间是MinValue，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsMinValue(this DateTime data, Action action)
        {
            if (data == DateTime.MinValue && action != null)
                action();
        }

        /// <summary>
        /// 当bool参数为true时，调用后续的比较函数。用于连续的条件比较。只要有一个为false，则返回false
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool TrueFunc(this bool flag, Func<bool> func)
        {
            bool result = flag;

            if (flag && func != null)
                result = func();

            return result;
        }

        /// <summary>
        /// 如果时间不是MinValue，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNotMinValue(this DateTime data, Action<DateTime> action)
        {
            if (data != DateTime.MinValue && action != null)
                action(data);
        }

        /// <summary>
        /// 比较两个对象的引用，如果都是null，返回true，如果有一个null，hasNull返回true
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="hasNull"></param>
        /// <returns></returns>
        public static bool ReferenceEqualWithNull(this object objA, object objB, out bool hasNull)
        {
            bool result = object.ReferenceEquals(objA, objB);

            if (objA == null || objB == null)
                hasNull = true;
            else
                hasNull = false;

            return result;
        }

        /// <summary>
        /// 对象类型是否是枚举类型，且TypeCode为Object
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static bool IsEnumerableObject(this object objectValue)
        {
            bool result = false;

            if (objectValue != null && Type.GetTypeCode(objectValue.GetType()) == TypeCode.Object)
                result = objectValue is IEnumerable;

            return result;
        }

        /// <summary>
        /// 得到某个枚举项的描述
        /// </summary>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static string ToDescription(this System.Enum enumItem)
        {
            enumItem.NullCheck("enumItem");

            return EnumItemDescriptionAttribute.GetDescription(enumItem);
        }
    }
}
