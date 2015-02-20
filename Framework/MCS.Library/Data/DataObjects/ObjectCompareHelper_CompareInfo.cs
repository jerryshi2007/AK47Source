using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using System.Reflection;

namespace MCS.Library.Data.DataObjects
{
    public static partial class ObjectCompareHelper
    {
        /// <summary>
        /// 根据类信息得到对象的比较信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ObjectCompareInfo GetCompareInfo<T>()
        {
            return GetCompareInfo(typeof(T));
        }

        /// <summary>
        /// 根据两个对象的类型（不为空的）得到比较信息
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="targetObject"></param>
        /// <returns>哪一个对象不为null，则返回哪个对象的type对应的CompareInfo，如果都不为空，则返回sourceObject的。如果都是空，则返回null</returns>
        public static ObjectCompareInfo GetCompareInfo(object sourceObject, object targetObject)
        {
            Type type = null;

            if (sourceObject != null)
                type = sourceObject.GetType();
            else
                if (targetObject != null)
                    type = targetObject.GetType();

            return type.GetCompareInfo();
        }

        /// <summary>
        /// 根据类信息得到对象的比较信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ObjectCompareInfo GetCompareInfo(this System.Type type)
        {
            ObjectCompareInfo result = null;

            if (type != null)
            {
                result = ObjectCompareInfoCache.Instance.GetOrAddNewValue(type, (cache, key) =>
                {
                    ObjectCompareInfo compareInfo = InnerGetCompareInfo(key);

                    cache.Add(key, compareInfo);

                    return compareInfo;
                });
            }

            return result;
        }

        /// <summary>
        /// 得到关键字的属性集合
        /// </summary>
        /// <param name="compareInfo"></param>
        /// <returns></returns>
        public static string[] GetCompareKeyFields(this IObjectCompareInfo compareInfo)
        {
            string[] result = StringExtension.EmptyStringArray;

            if (compareInfo != null)
            {
                if (compareInfo.KeyFields.IsNotEmpty())
                    result = compareInfo.KeyFields.Split(',', ';');
            }

            return result;
        }

        /// <summary>
        /// 将某一个对象的比较信息复制给另一个对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo(this IObjectCompareInfo source, IObjectCompareInfo target)
        {
            if (source != null && target != null)
            {
                target.IsList = source.IsList;
                target.KeyFields = source.KeyFields;
            }
        }

        /// <summary>
        /// 将某一个对象的属性比较信息复制给另一个对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo(this IPropertyCompareInfo source, IPropertyCompareInfo target)
        {
            if (source != null && target != null)
            {
                target.SortID = source.SortID;
                target.RequireCompare = source.RequireCompare;
                target.Description = source.Description;
            }
        }

        private static ObjectCompareInfo InnerGetCompareInfo(System.Type type)
        {
            ObjectCompareAttribute compareAttribute = AttributeHelper.GetCustomAttribute<ObjectCompareAttribute>(type);

            ObjectCompareInfo result = new ObjectCompareInfo();

            if (type != typeof(string))
                result.ObjectTypeName = type.AssemblyQualifiedName;

            compareAttribute.CopyTo(result);

            FillPropertiesCompareInfo(type, result);

            return result;
        }

        /// <summary>
        /// 填充属性的比较信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="compareInfo"></param>
        private static void FillPropertiesCompareInfo(System.Type type, ObjectCompareInfo compareInfo)
        {
            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo pi in pis)
            {
                AttributeHelper.GetCustomAttribute<PropertyCompareAttribute>(pi).IsNotNull(pca =>
                {
                    PropertyCompareInfo pci = new PropertyCompareInfo();

                    pci.PropertyName = pi.Name;

                    if (pi.PropertyType != typeof(string))
                        pci.PropertyTypeName = pi.PropertyType.AssemblyQualifiedName;

                    pca.CopyTo(pci);

                    compareInfo.Properties.Add(pci);
                });
            }
        }
    }
}
