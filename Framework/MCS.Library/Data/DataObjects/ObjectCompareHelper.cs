#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	ObjectCompareHelper.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张曦	    2008-03-17		创建
// -------------------------------------------------
#endregion

using MCS.Library.Core;
using MCS.Library.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 对象比较的集合类
    /// </summary>
    public static partial class ObjectCompareHelper
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="compareObject"></param>
        /// <returns></returns>
        public static ObjectCompareResult CompareObject(object sourceObject, object compareObject)
        {
            ObjectCompareInfo compareInfo = GetCompareInfo(sourceObject, compareObject);

            ObjectCompareResult result = null;

            if (compareInfo == null)
                result = new ObjectCompareResult();
            else
                result = CompareObject(compareInfo, sourceObject, compareObject);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareInfo"></param>
        /// <param name="sourceObject"></param>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public static ObjectCompareResult CompareObject(ObjectCompareInfo compareInfo, object sourceObject, object targetObject)
        {
            compareInfo.NullCheck("compareInfo");

            ObjectCompareResult result = new ObjectCompareResult(compareInfo.ObjectTypeName);

            //如果是同一个对象，则直接返回
            if (object.ReferenceEquals(sourceObject, targetObject))
                return result;

            foreach (PropertyCompareInfo pci in compareInfo.Properties)
            {
                object sourceValue = GetPropertyValue(sourceObject, pci.PropertyName);
                object targetValue = GetPropertyValue(targetObject, pci.PropertyName);

                if (object.Equals(sourceValue, targetValue) == false)
                {
                    ObjectPropertyCompareResult item = new ObjectPropertyCompareResult(pci, sourceValue, targetValue);

                    if (sourceValue.IsEnumerableObject() || targetValue.IsEnumerableObject())
                        item.SubObjectCompareResult = CompareEnumerableObject(sourceValue as IEnumerable, targetValue as IEnumerable);
                    else
                        item.SubObjectCompareResult = CompareObject(sourceValue, targetValue);

                    result.Add(item);
                }
            }

            result.Sort((r1, r2) => r1.SortID - r2.SortID);

            return result;
        }

        /// <summary>
        /// 比较两个集合对象
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public static ObjectCollectionCompareResult CompareEnumerableObject(IEnumerable sourceObject, IEnumerable targetObject)
        {
            ObjectCompareInfo collectionCompareInfo = GetCompareInfo(sourceObject, targetObject);

            string collectionTypeName = collectionCompareInfo != null ? collectionCompareInfo.ObjectTypeName : string.Empty;

            ObjectCollectionCompareResult result = new ObjectCollectionCompareResult(collectionTypeName);

            CompareEnumarableItemObject(sourceObject, targetObject, result.Updated, result.Deleted,
                (compareInfo, sourceValue, targetValue) => CompareObject(compareInfo, sourceValue, targetValue));

            CompareEnumarableItemObject(targetObject, sourceObject, null, result.Added,
                    (compareInfo, targetValue, sourceValue) => CompareObject(compareInfo, sourceValue, targetValue));

            return result;
        }

        private static void CompareEnumarableItemObject(IEnumerable sourceObject, IEnumerable targetObject,
            ObjectCompareResultCollection updated, ObjectCompareResultCollection addOrDeleted,
            Func<ObjectCompareInfo, object, object, ObjectCompareResult> createItemResult)
        {
            if (sourceObject != null)
            {
                foreach (object sourceValue in sourceObject)
                {
                    if (sourceValue != null)
                    {
                        ObjectCompareInfo compareInfo = sourceValue.GetType().GetCompareInfo();

                        string[] compareKeyFields = compareInfo.GetCompareKeyFields();

                        object targetValue = targetObject.FirstOrDefault<object>(t => IsSameCompareKey(compareInfo.GetCompareKeyFields(), sourceValue, t));

                        ObjectCompareResult itemResult = createItemResult(compareInfo, sourceValue, targetValue);

                        if (itemResult.AreDifferent)
                        {
                            ObjectCompareResultCollection resultCollection = GetCollectionByCompareValues(sourceValue, targetValue, updated, addOrDeleted);

                            if (resultCollection != null)
                                resultCollection.Add(itemResult);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据比较前后的对象决定使用哪个集合
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="updated"></param>
        /// <param name="addOrDeleted"></param>
        /// <returns></returns>
        private static ObjectCompareResultCollection GetCollectionByCompareValues(object sourceValue, object targetValue,
            ObjectCompareResultCollection updated, ObjectCompareResultCollection addOrDeleted)
        {
            ObjectCompareResultCollection result = updated;

            if (sourceValue == null && targetValue != null)   //没有原对象,Add
                result = addOrDeleted;
            else if (targetValue == null && sourceValue != null) //没有新对象,Delete
                result = addOrDeleted;

            return result;
        }

        private static bool IsSameCompareKey(IEnumerable<string> compareKeyFields, object sourceObject, object compareObject)
        {
            bool result = true;

            if (sourceObject != null && compareObject != null)
            {
                bool compared = false;
                foreach (string key in compareKeyFields)
                {
                    result = PropertyValueEquals(key, sourceObject, compareObject);

                    compared = true;

                    if (result == false)
                        break;
                }

                //如果没有比较过，则返回false
                if (compared == false)
                    result = false;
            }
            else
                result = false;

            return result;
        }

        /// <summary>
        /// 两个对象的属性值是否相同
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sourceObject"></param>
        /// <param name="compareObject"></param>
        /// <returns></returns>
        private static bool PropertyValueEquals(string propertyName, object sourceObject, object compareObject)
        {
            object sourceValue = GetPropertyValue(sourceObject, propertyName);
            object compareValue = GetPropertyValue(compareObject, propertyName);

            return object.Equals(sourceValue, compareValue);
        }

        private static object GetPropertyValue(object instance, string propertyName)
        {
            object result = null;

            if (instance != null)
                result = DynamicPropertyValueAccessor.Instance.GetValue(instance, propertyName);

            return result;
        }
    }
}
