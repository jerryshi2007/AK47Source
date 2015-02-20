using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MCS.Library.Core
{
    /// <summary>
    /// IEnumerable的接口扩展
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 判断集合中每个元素是否都满足某条件，且集合不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool AllAndNotEmpty<T>(this IEnumerable<T> data, Predicate<T> match)
        {
            bool result = true;
            bool notEmpty = false;

            if (data != null && match != null)
            {
                foreach (T item in data)
                {
                    notEmpty = true;

                    if (match(item) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result && notEmpty;
        }

        /// <summary>
        /// 枚举处理IEnumerable的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            if (data != null && action != null)
            {
                foreach (T item in data)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 枚举处理IEnumerable的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable data, Action<T> action)
        {
            if (data != null && action != null)
            {
                foreach (T item in data)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 枚举每一项存在的值，判断是否存在满足条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Exists<T>(this IEnumerable<T> data, Predicate<T> match)
        {
            bool result = false;

            if (data != null && match != null)
            {
                foreach (T item in data)
                {
                    if (match(item))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 枚举每一项存在的值，判断是否都不存在满足条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool NotExists<T>(this IEnumerable<T> data, Predicate<T> match)
        {
            return !Exists(data, match);
        }

        /// <summary>
        /// 枚举每一项存在的值，判断是否存在满足条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Exists<T>(this IEnumerable data, Predicate<T> match)
        {
            bool result = false;

            if (data != null && match != null)
            {
                foreach (T item in data)
                {
                    if (match(item))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 枚举每一项存在的值，判断是否都不存在满足条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool NotExists<T>(this IEnumerable data, Predicate<T> match)
        {
            return !Exists(data, match);
        }

        /// <summary>
        /// 得到符合条件的第一项，如果没有找到，则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this IEnumerable data, Predicate<T> match)
        {
            T result = default(T);

            if (data != null && match != null)
            {
                foreach (T item in data)
                {
                    if (match(item))
                    {
                        result = item;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 去除集合类中重复项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> data, EqualityComparerHandler<T> comparer)
        {
            GeneralEqualityComparer<T> comparerClass = new GeneralEqualityComparer<T>(comparer);

            return Enumerable.Distinct(data, comparerClass);
        }
    }
}
