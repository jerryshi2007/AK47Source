#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	ObjectCompareHelper.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    2008-03-17		����
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
    /// ����Ƚϵļ�����
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

            //�����ͬһ��������ֱ�ӷ���
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
        /// �Ƚ��������϶���
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
        /// ���ݱȽ�ǰ��Ķ������ʹ���ĸ�����
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

            if (sourceValue == null && targetValue != null)   //û��ԭ����,Add
                result = addOrDeleted;
            else if (targetValue == null && sourceValue != null) //û���¶���,Delete
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

                //���û�бȽϹ����򷵻�false
                if (compared == false)
                    result = false;
            }
            else
                result = false;

            return result;
        }

        /// <summary>
        /// �������������ֵ�Ƿ���ͬ
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
